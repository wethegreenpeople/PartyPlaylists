using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PartyPlaylists.Models.DataModels
{
    public class Token
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string JWTToken { get; set; }

        [JsonIgnore]
        public List<RoomSongToken> RoomSongTokens { get; set; }
    }
}
