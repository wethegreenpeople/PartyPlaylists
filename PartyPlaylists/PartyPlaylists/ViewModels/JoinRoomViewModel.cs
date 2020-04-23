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
using PartyPlaylists.Droid;

namespace PartyPlaylists.ViewModels
{
    public class JoinRoomViewModel : BaseViewModel
    {
        string _roomToJoin;
        public string RoomToJoin
        {
            get { return _roomToJoin; }
            set { SetProperty(ref _roomToJoin, value); }
        }

        public Command JoinRoomCommand { get; set; }

        public JoinRoomViewModel()
        {
            Title = "Join a Room";

            JoinRoomCommand = new Command(async () => await JoinRoom());
        }

        private async Task JoinRoom()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(RoomToJoin))
                return;

            IsBusy = true;

            try
            {
                var storedToken = await SecureStorage.GetAsync("jwtToken");
                if (storedToken == null || !TokenService.ValidateToken(storedToken, Keys.JwtKey))
                {
                    var token = await TokenService.CreateTokenAsync(Keys.JwtKey);
                    await SecureStorage.SetAsync("jwtToken", token);
                    storedToken = token;
                }

                var client = new RestClient(@"https://partyplaylists.azurewebsites.net");
                var request = new RestRequest($@"api/room/{RoomToJoin}", Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", $"Bearer {storedToken}");

                var response = await client.ExecuteAsync(request);
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