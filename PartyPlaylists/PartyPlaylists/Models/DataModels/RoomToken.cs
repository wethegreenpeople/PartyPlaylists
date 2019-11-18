using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Models.DataModels
{
    public class RoomToken
    {
        public int TokenId { get; set; }
        public Token Token { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public List<Song> SongsVotedOn { get; set; }
    }
}
