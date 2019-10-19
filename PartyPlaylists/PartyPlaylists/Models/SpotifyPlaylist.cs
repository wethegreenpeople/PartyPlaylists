using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Models
{
    public class SpotifyPlaylist : IPlaylist
    {
        public string PlaylistID { get; set; }
        public string PlaylistName { get; set; }
        public string PlaylistOwnerID { get; set; }
    }
}
