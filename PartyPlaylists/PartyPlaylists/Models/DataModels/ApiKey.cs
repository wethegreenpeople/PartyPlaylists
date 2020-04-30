using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PartyPlaylists.Models.DataModels
{
    public class ApiKey
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string User { get; set; }
    }
}
