using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using SpotifyApi.NetCore.Authorization;
using PartyPlaylists.MVC.Hubs;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomDataStore _roomDataStore;
        private readonly TokenService _tokenService;
        private readonly SpotifyPlaylistsStore _spotifyPlaylistsStore;
        private readonly IConfiguration _config;
        public RoomController(IConfiguration config, PlaylistContext playlistContext)
        {
            _roomDataStore = new RoomDataStore(playlistContext);
            _tokenService = new TokenService(playlistContext, config);
            _spotifyPlaylistsStore = new SpotifyPlaylistsStore(playlistContext);
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

            var spotify = new SpotifyService(room.SpotifyAuthCode);
            await _roomDataStore.RemovePreviouslyPlayedSongsAsync(room.Id);
            await spotify.ReorderPlaylist(room.SpotifyPlaylist, room);

            if (room != null)
            {
                await new RoomHub().UpdateSongsAsync(roomId.ToString());
                return PartialView("Components/_roomSongTableRow", room.RoomSongs);
            }

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

            var playlist = await _spotifyPlaylistsStore.GetItemByRoomId(room.Id.ToString());
            await spotify.AddSongToPlaylist(playlist, song);

            await _roomDataStore.RemovePreviouslyPlayedSongsAsync(room.Id);

            return PartialView("Components/_roomSongTableRow", room.RoomSongs);
        }

        [HttpGet]
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
            var refreshToken = (await new UserAccountsService(new HttpClient(), _config).RequestAccessRefreshToken(code)).AccessToken;
            var room =  await _roomDataStore.GetItemAsync(state);

            room.SpotifyAuthCode = refreshToken;
            var request = HttpContext.Request;
            string roomUrl = $"{request.Scheme}://{request.Host}/Room/Index/{room.Id}";

            var spotify = new SpotifyService(refreshToken);
            var spotifyUserId = await spotify.GetUserIdAsync();

            var updateRoomTask = _roomDataStore.UpdateItemAsync(room);
            var createSpotifyRoomTask = spotify.CreatePlaylist(room.Name, spotifyUserId, roomUrl);

            await Task.WhenAll(updateRoomTask, createSpotifyRoomTask);

            var playlist = (SpotifyPlaylist)createSpotifyRoomTask.Result;
            playlist.Room = room;
            await _spotifyPlaylistsStore.AddItemAsync(playlist);

            return RedirectToAction("Index", "Room", routeValues: new { Id = state });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomSongs(int roomId)
        {
            var room = await _roomDataStore.GetItemAsync(roomId.ToString());

            return PartialView("Components/_roomSongTableRow", room.RoomSongs);
        }

        [HttpGet]
        public async Task<IActionResult> LiveSearch(string auth, string query)
        {
            var spotifyService = new SpotifyService(auth);
            try
            {
                var results = (await spotifyService.GetSongs(query)).Take(5).ToList();
                return PartialView("Components/_songSearchItems", new RoomVM() { SearchedSongs = results});
            }
            catch
            {
                return BadRequest();
            }

        }
    }
}