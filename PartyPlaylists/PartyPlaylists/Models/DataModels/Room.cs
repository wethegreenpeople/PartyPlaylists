using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.Models.DataModels
{
    public class Room
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Owner { get; set; }

        public string Name { get; set; }

        public List<RoomSong> RoomSongs { get; set; }

        public bool IsSpotifyEnabled { get; set; } = false; // TODO: Might not be necessary. Just check for presence of auth code?

        public string SpotifyAuthCode { get; set; }

        public bool AllowTransferOfControl { get; set; }

        // TODO: Investigate why this is here
        // Genuinally don't remember what this is supposed to achieve, I think it was for the webAPI
        // we can probably remove it
        [JsonIgnore]
        public SpotifyPlaylist SpotifyPlaylist { get; set; }
    }
}
