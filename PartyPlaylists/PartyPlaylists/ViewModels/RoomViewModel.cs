using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using RestSharp;
using Splat;
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
    public class RoomViewModel : BaseViewModel
    {
        private readonly HubConnection _hubConnection = Locator.Current.GetService<HubConnection>();
        private readonly ISpotifyMobileSDK _spotifyMobileSDK = Locator.Current.GetService<ISpotifyMobileSDK>();
        private readonly RestClient _partyPlaylistsClient;

        string _roomName;
        public string RoomName
        {
            get { return _currentRoom?.Name?.ToUpper(); }
            set { SetProperty(ref _roomName, value); }
        }

        string _songName;
        public string SongName
        {
            get { return _songName; }
            set { SetProperty(ref _songName, value); }
        }

        bool _showAuthenticateButton = true;
        public bool ShowAuthenticateButton
        {
            get { return _showAuthenticateButton; }
            set { SetProperty(ref _showAuthenticateButton, value); }
        }

        bool _showPlayButton = false;
        public bool ShowPlayButton
        {
            get { return _showPlayButton; }
            set { SetProperty(ref _showPlayButton, value); }
        }

        ObservableCollection<RoomSong> _roomSongs;
        public ObservableCollection<RoomSong> RoomSongs
        {
            get { return _roomSongs; }
            set { SetProperty(ref _roomSongs, value); }
        }

        ObservableCollection<Song> _searchedSongs;
        public ObservableCollection<Song> SearchedSongs
        {
            get { return _searchedSongs; }
            set { SetProperty(ref _searchedSongs, value); }
        }

        Room _currentRoom;
        public Room CurrentRoom
        {
            get { return _currentRoom; }
            set { SetProperty(ref _currentRoom, value); }
        }

        public Command AddVoteCommand { get; set; }
        public Command SearchForSongCommand { get; set; }
        public Command AddSongToRoomCommand { get; set; }
        public Command AuthorizeSpotifyCommand { get; set; }
        public Command StartPlaylistCommand { get; set; }

        public RoomViewModel()
        {
            Title = "Room"; 
            AddVoteCommand = new Command<int>(async (int songId) => await AddVote(songId));
            SearchForSongCommand = new Command(async () => await SearchForSong());
            AddSongToRoomCommand = new Command<Song>(async (Song songToAdd) => await AddSongToRoom(songToAdd));
            AuthorizeSpotifyCommand = new Command(async () => await AuthorizeSpotify());
            StartPlaylistCommand = new Command(async () => await StartPlaylist());
        }

        public RoomViewModel(Room room) : this()
        {
            CurrentRoom = room;
            if (CurrentRoom.RoomSongs != null)
                RoomSongs = new ObservableCollection<RoomSong>(CurrentRoom.RoomSongs.OrderByDescending(s => s.SongRating).ToList());
            _partyPlaylistsClient = new RestClient(@"https://partyplaylists.azurewebsites.net");

            if (_hubConnection.State == HubConnectionState.Disconnected)
                _hubConnection.StartAsync();

            // TODO: Ensure that these don't trigger on any signlarr update (updates outside of the current room)
            // and if they do, that we don't take action upon them
            _hubConnection.On<string>("Update", async (roomId) =>
            {
                await UpdateRoomSongs(roomId);
            });

            _hubConnection.On<string>("PlayNextSong", async (roomId) =>
            {
                await RemoveSong(roomId);
                await UpdateRoomSongs(roomId);
                await _hubConnection.InvokeAsync("UpdateSongsAsync", CurrentRoom.Id.ToString());
                await StartPlaylist();
            });
        }

        private async Task UpdateRoomSongs(string roomId)
        {
            var storedToken = await SecureStorage.GetAsync("jwtToken");
            var request = new RestRequest($@"api/room/{roomId}", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"Bearer {storedToken}");
            RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>((await _partyPlaylistsClient.ExecuteAsync(request)).Content).RoomSongs.OrderByDescending(s => s.SongRating));
        }

        private async Task RemoveSong(string roomId)
        {
            var topSongId = RoomSongs.OrderByDescending(s => s.SongRating).FirstOrDefault().SongId;
            var storedToken = await SecureStorage.GetAsync("jwtToken");
            var request = new RestRequest($@"api/room/{roomId}/remove/{topSongId}", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"Bearer {storedToken}");
            var results = await _partyPlaylistsClient.ExecuteAsync(request);
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
            }   
        }

        private async Task SearchForSong()
        {
            if (string.IsNullOrEmpty(SongName))
                return;

            IsBusy = true;

            var jwtToken = await SecureStorage.GetAsync("jwtToken");
            var request = new RestRequest($@"api/Song/{SongName}", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"Bearer {jwtToken}");

            try
            {
                var response = await _partyPlaylistsClient.ExecuteAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SearchedSongs = new ObservableCollection<Song>(JsonConvert.DeserializeObject<List<Song>>(response.Content));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddSongToRoom(Song songToAdd)
        {
            if (string.IsNullOrEmpty(SongName))
                return;

            var jwtToken = await SecureStorage.GetAsync("jwtToken");
            var request = new RestRequest($@"api/Room/{CurrentRoom.Id}", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"Bearer {jwtToken}");
            request.AddJsonBody(songToAdd);

            var response = await _partyPlaylistsClient.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>((await _partyPlaylistsClient.ExecuteAsync(request)).Content).RoomSongs.OrderByDescending(s => s.SongRating));
                SearchedSongs.Remove(songToAdd);
                await _hubConnection.InvokeAsync("UpdateSongsAsync", CurrentRoom.Id.ToString());
            }
        }

        private async Task AuthorizeSpotify()
        {
            await _spotifyMobileSDK.Authenticate(); // TODO: Figure out how to hook into the onconnected event 
            ShowAuthenticateButton = false;
            ShowPlayButton = true;
        }
        private async Task StartPlaylist()
        {
            if (RoomSongs.Count > 0)
                _spotifyMobileSDK.PlaySong(RoomSongs.OrderByDescending(s => s.SongRating).FirstOrDefault().Song.ServiceId, CurrentRoom.Id.ToString());
        }
    }
}
