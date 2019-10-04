using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.Models.DataModels
{
    public class RoomSong
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }
        public int SongRating { get; set; }
    }
}
