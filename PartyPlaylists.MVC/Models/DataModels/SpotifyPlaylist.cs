using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace PartyPlaylists.MVC.Models.DataModels
{
    public class SpotifyPlaylist : IPlaylist
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string AuthCode { get; set; }

        public string PlaylistID { get; set; }
        public string PlaylistName { get; set; }
        public string PlaylistOwnerID { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
