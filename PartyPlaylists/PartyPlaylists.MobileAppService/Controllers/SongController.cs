using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;

namespace PartyPlaylists.MobileAppService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly PlaylistContext _context;

        public SongController(PlaylistContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Song>> GetSingleSong(int id)
        {
            var song = await _context.Songs.SingleOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound();
            return song;
        }
    }
}