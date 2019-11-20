using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Models.DataModels
{
    public class RoomSongToken
    {
        public int Id { get; set; }

        public int TokenId { get; set; }
        public Token Token { get; set; }

        public int RoomSongId { get; set; }
        public RoomSong RoomSong { get; set; }
    }
}
