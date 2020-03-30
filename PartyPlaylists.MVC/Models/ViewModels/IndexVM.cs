using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MVC.Models.ViewModels
{
    public class IndexVM
    {
        [DisplayName("Room to Join")]
        [MinLength(1)]
        public string RoomToJoin { get; set; }

        [DisplayName("Room Name")]
        public string NewRoomName { get; set; }
    }
}
