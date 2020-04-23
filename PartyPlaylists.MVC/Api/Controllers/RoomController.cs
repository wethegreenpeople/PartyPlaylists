using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public RoomController(PlaylistContext context)
        {
            _playlistContext = context;
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
    }
}