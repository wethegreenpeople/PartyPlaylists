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

namespace PartyPlaylists.MVC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly PlaylistContext _playlistContext;
        private readonly IConfiguration _config;

        public SongController(PlaylistContext context, IConfiguration config)
        {
            _playlistContext = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("{songQuery}")]
        public async Task<List<Song>> GetSongs(string songQuery)
        {
            var spotifyService = new SpotifyService();
            var songs = await spotifyService.GetSongs(songQuery);
            return songs.Take(5).ToList();
        }
    }
}