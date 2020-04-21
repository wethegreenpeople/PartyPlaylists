using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.ViewModels
{
    public class RoomViewModel : BaseViewModel
    {
        public readonly Room CurrentRoom;

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
