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
    public class RoomViewModel : BaseViewModel
    {
        string _roomName;
        public string RoomName
        {
            get { return _currentRoom?.Name?.ToUpper(); }
            set { SetProperty(ref _roomName, value); }
        }

        ObservableCollection<RoomSong> _roomSongs;
        public ObservableCollection<RoomSong> RoomSongs
        {
<<<<<<< HEAD
            get { return _roomSongs; }
=======
            get { return new ObservableCollection<RoomSong>(_currentRoom?.RoomSongs.OrderByDescending(s => s.SongRating).ToList()); }
>>>>>>> 4fa17c2... Add unique validation while voting for a song (one vote per token)
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
<<<<<<< HEAD
                RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>(response.Content).RoomSongs.OrderByDescending(s => s.SongRating));
=======
                RoomSongs = new ObservableCollection<RoomSong>(JsonConvert.DeserializeObject<Room>(response.Content).RoomSongs);
>>>>>>> 4fa17c2... Add unique validation while voting for a song (one vote per token)
            }
        }
    }
}
