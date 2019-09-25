﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MobileAppService.Models.DataModels
{
    public class Room
    {
        [Key]
        [Required]
        [JsonIgnore]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Owner { get; set; }

        public string Name { get; set; }

        public List<RoomSong> RoomSongs { get; set; } = new List<RoomSong>();
    }
}
