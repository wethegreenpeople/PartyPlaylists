using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MVC.Models.ViewModels
{
    public class IndexVM
    {
        [DisplayName("Room to Join")]
        public string RoomToJoin { get; set; }

        [DisplayName("Room Name")]
        public string NewRoomName { get; set; }
    }
}
