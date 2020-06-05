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
    public class CreateRoomViewModel : BaseViewModel
    {
        private readonly RestClient _partyPlaylistsClient;
        private readonly Task<ICustomConfig> _config;

        string _roomName;
        public string RoomName
        {
            get { return _roomName; }
            set { SetProperty(ref _roomName, value); }
        }

        public Command CreateRoomCommand { get; set; }

        public CreateRoomViewModel()
        {
            _partyPlaylistsClient = new RestClient(@"https://partyplaylists.azurewebsites.net");

            Title = "Create a Room";
            CreateRoomCommand = new Command(async () => await CreateRoom());

            var fileStorage = Locator.Current.GetService<IFileStorage>();
            _config = Locator.Current.GetService<ICustomConfig>().Build(fileStorage);
        }

        private async Task CreateRoom()
        {
            async Task<string> GetNewToken()
            {
                var devAuth = (await _config).PartyPlaylistsKey;
                var tokenRequest = new RestRequest($@"api/Token/{devAuth}", Method.POST);
                var response = await _partyPlaylistsClient.ExecuteAsync(tokenRequest);
                var token = response.Content.Trim('"');
                await SecureStorage.SetAsync("jwtToken", token);
                return token;
            }

            async Task<IRestResponse> CreateRoom(string token)
            {
                var request = new RestRequest($@"api/room/{RoomName}", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", $"Bearer {token}");

                return await _partyPlaylistsClient.ExecuteAsync(request);
            }

            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(RoomName))
                return;

            IsBusy = true;

            try
            {
                var storedToken = await SecureStorage.GetAsync("jwtToken");
                if (string.IsNullOrEmpty(storedToken))
                    storedToken = await GetNewToken();

                IRestResponse response; 
                response = await CreateRoom(storedToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    SecureStorage.Remove("jwtToken");
                    var newToken = await GetNewToken();
                    response = await CreateRoom(newToken);
                }
                    
                var content = response.Content;
                var room = JsonConvert.DeserializeObject<Room>(content);

                await RootPage.Detail.Navigation.PushAsync(new RoomTabbedPage(room));
            }
            catch (Exception ex)
            {
            }
            
            IsBusy = false;
        }
    }
}