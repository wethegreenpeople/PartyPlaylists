using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        List<RoomSong> _roomSongs;
        public List<RoomSong> RoomSongs
        {
            get { return _currentRoom?.RoomSongs.OrderByDescending(s => s.SongRating)?.ToList(); }
            set { SetProperty(ref _roomSongs, value); }
        }

        Room _currentRoom;
        public Room CurrentRoom
        {
            get { return _currentRoom; }
            set { SetProperty(ref _currentRoom, value); }
        }

        public RoomViewModel()
        {
            Title = "Room";
        }

        public RoomViewModel(Room room) : this()
        {
            CurrentRoom = room;
        }
    }
}
