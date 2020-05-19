using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using SpotifyApi.NetCore;

namespace PartyPlaylists.MVC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly PlaylistContext _playlistContext;
        private readonly IConfiguration _config;

        public RoomController(PlaylistContext context, IConfiguration config)
        {
            _playlistContext = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("{roomId}", Name = "Get")]
        public async Task<Room> GetRoom(int roomId)
        {
            var roomDataStore = new RoomDataStore(_playlistContext);
            return await roomDataStore.GetItemAsync(roomId.ToString());
        }

        [Authorize]
        [HttpPost("{roomId}/{songId}")]
        public async Task<Room> AddVoteToSong(int roomId, int songId)
        {
            var roomDataStore = new RoomDataStore(_playlistContext);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return await roomDataStore.AddVoteToSong(token, roomId, songId, 1);
        }

        [Authorize]
        [HttpPost("{roomId:int}", Name = "AddSong")]
        public async Task<Room> AddSongToRoom(int roomId, [FromBody]Song songToAdd)
        {
            var roomDataStore = new RoomDataStore(_playlistContext);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var usernameAddingSong = new TokenService(_playlistContext, _config).GetNameFromToken(token);
            return await roomDataStore.AddSongToRoomAsync(usernameAddingSong, roomId.ToString(), songToAdd);
        }

        [Authorize]
        [HttpPost("{roomName}")]
        public async Task<Room> CreateRoom(string roomName)
        {
            var roomDataStore = new RoomDataStore(_playlistContext);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var room = new Room()
            {
                Name = roomName,
                Owner = new TokenService(_playlistContext, _config).GetNameFromToken(token),
                IsSpotifyEnabled = false,
            };
            return await roomDataStore.AddItemAsync(room, token);
        }
    }
}