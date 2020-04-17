using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyPlaylists.Models.DataModels
{
    public class Song
    {
        [JsonIgnore]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Artist { get; set; }

        public string AlbumArt { get; set; }

        [Required]
        public StreamingServiceTypes ServiceAvailableOn { get; set; }

        [Required]
        public string ServiceId { get; set; }

        [JsonIgnore]
        public List<RoomSong> RoomSongs { get; set; } = new List<RoomSong>();
    }
}
