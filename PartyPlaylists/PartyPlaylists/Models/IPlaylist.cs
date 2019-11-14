using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Models
{
    public interface IPlaylist
    {
        string PlaylistOwnerID { get; set; }
        string PlaylistID { get; set; }
        string PlaylistName { get; set; }
        string AuthCode { get; set; }
    }
}
