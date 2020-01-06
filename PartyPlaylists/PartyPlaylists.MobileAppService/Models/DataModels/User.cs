using System;
using Newtonsoft.Json;
using PartyPlaylists.MobileAppService.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartyPlaylists.MobileAppService.Models.DataModels
{
    public class User
    {
        [Required]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public int ServiceEnabled { get; set; } 

    }
}
