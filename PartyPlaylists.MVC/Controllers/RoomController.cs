using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
using Microsoft.AspNetCore.Http;
using SpotifyApi.NetCore.Authorization;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomDataStore _roomDataStore;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;

        public RoomController(IConfiguration config, PlaylistContext playlistContext)
        {
            _roomDataStore = new RoomDataStore(playlistContext);
            _tokenService = new TokenService(playlistContext, config);
            _config = config;
        }

        public async Task<IActionResult> Index(string Id)
        {
            Request.Cookies.TryGetValue("jwtToken", out string jwtToken);
            if (string.IsNullOrEmpty(jwtToken))
            {
                var option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                jwtToken = await _tokenService.GetToken();
                Response.Cookies.Append("jwtToken", jwtToken, option);
            }
            var roomVm = new RoomVM()
            {
                CurrentRoom = await _roomDataStore.GetItemAsync(Id),
                JwtToken = jwtToken,
            };

            return View("Index", roomVm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddVoteToSong(int roomId, int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var room = await (_roomDataStore).AddVoteToSong(token, roomId, songId, 1);
            var updatedSongInfo = room.RoomSongs.Where(s => s.SongId == songId).Select(s => new { Id = songId, Rating = s.SongRating });
            if (room != null)
                return Json(
                    JsonConvert.SerializeObject(
                        updatedSongInfo, 
                        Formatting.None,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            return StatusCode(500);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSongToRoom(RoomVM roomVM)
        {
            var spotify = new SpotifyService(roomVM.CurrentRoom.SpotifyAuthCode);
            var song = await spotify.GetSong(roomVM.SongToAdd);

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var username = _tokenService.GetNameFromToken(token);
            var room = await _roomDataStore.AddSongToRoomAsync(username, roomVM.CurrentRoom.Id.ToString(), song);

            return PartialView("Components/_roomSongTableRow", room.RoomSongs.Single(s => s.SongId == song.Id));
        }

        
        public IActionResult AuthorizeSpotify(RoomVM roomVM)
        {
            var accountService = new UserAccountsService(new HttpClient(), _config);

            string url = accountService
                .AuthorizeUrl(
                    roomVM.CurrentRoom.Id.ToString(), 
                    new[] { "user-read-playback-state streaming user-read-private user-read-email playlist-read-private user-library-read playlist-modify-public playlist-modify-private" });

            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> SpotifyAuthorized(string code, string state)
        {
            var room =  await _roomDataStore.GetItemAsync(state);
            room.SpotifyAuthCode = code;
            await _roomDataStore.UpdateItemAsync(room);

            return RedirectToAction("Index", "Room", routeValues: new { Id = state });
        }
    }
}