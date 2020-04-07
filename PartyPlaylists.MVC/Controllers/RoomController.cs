using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;
using Microsoft.Extensions.Configuration;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomDataStore _roomDataStore;
        private readonly TokenService _tokenService;

        public RoomController(IConfiguration config, PlaylistContext playlistContext)
        {
            _roomDataStore = new RoomDataStore(playlistContext);
            _tokenService = new TokenService(playlistContext, config);
        }

        public async Task<IActionResult> Index(string Id)
        {
            var roomVm = new RoomVM()
            {
                CurrentRoom = await _roomDataStore.GetItemAsync(Id),
                JwtToken = await _tokenService.GetToken(),
            };

            return View("Index", roomVm);
        }

        [Authorize]
        public async Task<IActionResult> AddVoteToSong(int roomId, int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var room = await (_roomDataStore).AddVoteToSong(token, roomId, songId, 1);
            var updatedSongInfo = room.RoomSongs.Where(s => s.SongId == songId).Select(s => new { Id = songId, Rating = s.SongRating +  1 }); // Not returning the updated room?
            if (room != null)
                return Json(
                    JsonConvert.SerializeObject(
                        updatedSongInfo, 
                        Formatting.None,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            return StatusCode(500);
        }
    }
}