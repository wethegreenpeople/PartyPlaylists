using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PartyPlaylists.ViewModels
{
    public class RoomViewModel : BaseViewModel, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly RestClient _partyPlaylistsClient;

        string _roomName;
        public string RoomName
        {
            get { return _currentRoom?.Name?.ToUpper(); }
            set { SetProperty(ref _roomName, value); }
        }

        ObservableCollection<RoomSong> _roomSongs;
        public ObservableCollection<RoomSong> RoomSongs
        {
            get { return _roomSongs; }
            set { SetProperty(ref _roomSongs, value); }
        }

        Room _currentRoom;
        public Room CurrentRoom
        {
            get { return _currentRoom; }
            set { SetProperty(ref _currentRoom, value); }
        }

        public Command AddVoteCommand { get; set; }

        public RoomViewModel()
        {
            Title = "Room"; 
            AddVoteCommand = new Command<int>(async (int songId) => await AddVote(songId));
        }

        public RoomViewModel(Room room) : this()
        {
            CurrentRoom = room;
            RoomSongs = new ObservableCollection<RoomSong>(CurrentRoom.RoomSongs.OrderByDescending(s => s.SongRating).ToList());
            _partyPlaylistsClient = new RestClient(@"https://partyplaylists.azurewebsites.net");
            _hubConnection = new HubConnectionBuilder().WithUrl($"https://partyplaylists.azurewebsites.net/roomhub")
                                                       .Build();
            _hubConnection.StartAsync();
            _hubConnection.On<string>("Update", async (roomId) =>
            {
                var storedToken = await SecureStorage.GetAsync("jwtToken");
                var request = new RestRequest($@"api/room/{roomId}", Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", $"Bearer {storedToken}");
                RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>((await _partyPlaylistsClient.ExecuteAsync(request)).Content).RoomSongs.OrderByDescending(s => s.SongRating));
            });
        }

        private async Task AddVote(int songId)
        {
            if (CurrentRoom == null)
                return;

            var jwtToken = await SecureStorage.GetAsync("jwtToken");
            var client = new RestClient(@"https://partyplaylists.azurewebsites.net");
            var request = new RestRequest($@"api/room/{CurrentRoom.Id}/{songId}", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"Bearer {jwtToken}");

            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>(response.Content).RoomSongs.OrderByDescending(s => s.SongRating));

                try
                {
                    await _hubConnection.InvokeAsync("UpdateSongsAsync", CurrentRoom.Id.ToString());
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await _hubConnection.StopAsync();
                }
            }   
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual async void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                await _hubConnection.StopAsync();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RoomViewModel()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
