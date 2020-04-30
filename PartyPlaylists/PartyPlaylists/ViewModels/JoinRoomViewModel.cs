using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using PartyPlaylists.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Splat;
using Microsoft.EntityFrameworkCore;
using SpotifyApi.NetCore;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Net;

namespace PartyPlaylists.ViewModels
{
    public class JoinRoomViewModel : BaseViewModel
    {
        private readonly RestClient _partyPlaylistsClient;

        string _roomToJoin;
        public string RoomToJoin
        {
            get { return _roomToJoin; }
            set { SetProperty(ref _roomToJoin, value); }
        }

        public Command JoinRoomCommand { get; set; }

        public JoinRoomViewModel()
        {
            _partyPlaylistsClient = new RestClient(@"https://localhost:5001");

            Title = "Join a Room";
            JoinRoomCommand = new Command(async () => await JoinRoom());
        }

        private async Task JoinRoom()
        {
            async Task<string> GetNewToken()
            {
                var devAuth = "<YOUR_DEV_AUTH_CODE_HERE>";
                var tokenRequest = new RestRequest($@"api/Token/{devAuth}", Method.POST);
                var response = await _partyPlaylistsClient.ExecuteAsync(tokenRequest);
                var token = response.Content;
                await SecureStorage.SetAsync("jwtToken", token);
                return token;
            }

            async Task<IRestResponse> GetRoom(string token)
            {
                var request = new RestRequest($@"api/room/{RoomToJoin}", Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", $"Bearer {token}");

                return await _partyPlaylistsClient.ExecuteAsync(request);
            }

            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(RoomToJoin))
                return;

            IsBusy = true;

            try
            {
                var storedToken = await SecureStorage.GetAsync("jwtToken");
                if (string.IsNullOrEmpty(storedToken))
                    storedToken = await GetNewToken();

                IRestResponse response; 
                response = await GetRoom(storedToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    SecureStorage.Remove("jwtToken");
                    var newToken = await GetNewToken();
                    response = await GetRoom(newToken);
                }
                    
                var content = response.Content;
                var room = JsonConvert.DeserializeObject<Room>(content);

                await RootPage.Detail.Navigation.PushAsync(new RoomPage(room));
            }
            catch (Exception ex)
            {
            }
            
            IsBusy = false;
        }
    }
}