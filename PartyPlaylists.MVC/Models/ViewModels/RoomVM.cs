using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MVC.Models.ViewModels
{
    public class RoomVM
    {
        public Room CurrentRoom { get; set; }
        public string JwtToken { get; set; }
        public string SongToAdd { get; set; }
        public List<Song> SearchedSongs { get; set; }
    }
}
