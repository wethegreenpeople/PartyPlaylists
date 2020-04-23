﻿using PartyPlaylists.Contexts;
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

namespace PartyPlaylists.ViewModels
{
    public class JoinRoomViewModel : BaseViewModel
    {
        private readonly RoomDataStore _roomDataStore;

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
                var client = new RestClient(@"https://partyplaylists.azurewebsites.net");
                var request = new RestRequest($@"api/room/{RoomToJoin}", Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJTYWJlciBEaXJ0IiwibmJmIjoxNTg3NTgyNDU1LCJleHAiOjE1ODc2Njg4NTUsImlhdCI6MTU4NzU4MjQ1NX0.5aZR2YrcJx45YSTHmoUYOqYiR-xPNKJWarrDSnVOIjo");

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