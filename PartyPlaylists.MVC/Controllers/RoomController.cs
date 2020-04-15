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
using Microsoft.AspNetCore.SignalR;
using System.Resources;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomDataStore _roomDataStore;
        private readonly TokenService _tokenService;
        private readonly SpotifyPlaylistsStore _spotifyPlaylistsStore;
        private readonly IConfiguration _config;
        private readonly IHubContext<RoomHub> _roomHub;

        public RoomController(IConfiguration config, PlaylistContext playlistContext, IHubContext<RoomHub> roomHub)
        {
            _roomDataStore = new RoomDataStore(playlistContext);
            _tokenService = new TokenService(playlistContext, config);
            _spotifyPlaylistsStore = new SpotifyPlaylistsStore(playlistContext);
            _config = config;
            _roomHub = roomHub;
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
                CurrentUserName = _tokenService.GetNameFromToken(jwtToken),
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
            try
            {
                var removeTask = _roomDataStore.RemovePreviouslyPlayedSongsAsync(room.Id);
                var reorderTask = spotify.ReorderPlaylist(room.SpotifyPlaylist, room);

                await Task.WhenAll(removeTask, reorderTask);
            }
            catch { }

            if (room != null)
            {
                await _roomHub.Clients.All.SendAsync("Update", roomId.ToString());
                return PartialView("Components/_roomSongListItem", room.RoomSongs);
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
            Room room = null;
            try
            {
                room = await _roomDataStore.AddSongToRoomAsync(username, roomVM.CurrentRoom.Id.ToString(), song);

                var playlist = await _spotifyPlaylistsStore.GetItemByRoomId(room.Id.ToString());
                await spotify.AddSongToPlaylist(playlist, song);

                await _roomDataStore.RemovePreviouslyPlayedSongsAsync(room.Id);
            }
            catch { }
            

            if (room != null)
            {
                await _roomHub.Clients.All.SendAsync("Update", room.Id.ToString());
                return PartialView("Components/_roomSongListItem", room.RoomSongs);
            }

            return PartialView("Components/_roomSongListItem", room.RoomSongs);
        }

        [HttpGet]
        public IActionResult AuthorizeSpotify(RoomVM roomVM)
        {
            var accountService = new UserAccountsService(new HttpClient(), _config);

            var roomId = roomVM.SyncAuthorization ? $"{roomVM.CurrentRoom.Id}-sync" : roomVM.CurrentRoom.Id.ToString();
            string url = accountService
                .AuthorizeUrl(
                    roomId, 
                    new[] { "user-read-playback-state streaming user-read-private user-read-email playlist-read-private user-library-read playlist-modify-public playlist-modify-private" });
            if (roomVM.SyncAuthorization)
                return Ok(url);

            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> SpotifyAuthorized(string code, string state)
        {
            if (state.Contains("-sync"))
            {
                var option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                Request.Cookies.TryGetValue("jwtToken", out string jwtToken);
                jwtToken = await _tokenService
                    .AddClaimToToken(
                        jwtToken,
                        "spotifyAuth",
                        (await new UserAccountsService(new HttpClient(), _config).RequestAccessRefreshToken(code)).AccessToken);
                Response.Cookies.Append("jwtToken", jwtToken, option);

                var roomId = state.TrimEnd("-sync".ToCharArray());
                return RedirectToAction("Index", "Room", routeValues: new { Id = roomId });
            }

            var refreshToken = (await new UserAccountsService(new HttpClient(), _config).RequestAccessRefreshToken(code)).AccessToken;
            var room =  await _roomDataStore.GetItemAsync(state);

            room.SpotifyAuthCode = refreshToken;
            var request = HttpContext.Request;
            var roomUrl = $"{request.Scheme}://{request.Host}/Room/Index/{room.Id}";

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

            return PartialView("Components/_roomSongListitem", room.RoomSongs);
        }

        [HttpGet]
        public async Task<IActionResult> GetNextSongToPlay(int roomId, string songUri)
        {
            await _roomDataStore.UpdatePreviouslyPlayedSongs(roomId);
            await _roomDataStore.RemovePreviouslyPlayedSongsAsync(roomId);
            var room = await _roomDataStore.GetItemAsync(roomId.ToString());
            var nextSong = room.RoomSongs
                .Where(s => s.Song.SpotifyId != songUri)
                .OrderByDescending(s => s.SongRating)
                .ElementAt(0)
                .Song
                .SpotifyId;

            return Json(nextSong);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateTransferOfControl(int roomId, bool allowTransferOfControl)
        {
            try
            {
                await _roomDataStore.UpdateTransferOfAudioControlAsync(roomId, allowTransferOfControl);
                await _roomHub.Clients.All.SendAsync("AllowTransfer", allowTransferOfControl);
                return RedirectToAction("Index", "Room", routeValues: new { Id = roomId });
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkCurrentSongAsPlayed(int roomId)
        {
            try
            {
                await _roomDataStore.UpdatePreviouslyPlayedSongs(roomId);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentSongPosistion(int roomId)
        {
            try
            {
                var room = await _roomDataStore.GetItemAsync(roomId.ToString());
                var spotify = new SpotifyService(room.SpotifyAuthCode);
                var currentSong = await spotify.GetCurrentlyPlayingSongAsync();
                return Ok(currentSong.ProgressMs);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}