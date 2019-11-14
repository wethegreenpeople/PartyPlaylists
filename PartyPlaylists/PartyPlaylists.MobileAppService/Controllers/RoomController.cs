using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SystemWeb = System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyPlaylists.MobileAppService.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Models;
using PartyPlaylists.Services;

namespace PartyPlaylists.MobileAppService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly PlaylistContext _context;

        public RoomController(PlaylistContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Room>>> GetAllRooms() => await _context.Rooms.Include(e => e.RoomSongs).ToListAsync();

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Room>> GetSingleRoom(string id)
        {
            var room = await _context.Rooms
                .Include(e => e.RoomSongs)
                .SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

            if (room == null)
                return NotFound();

            foreach (var roomsong in room.RoomSongs)
            {
                roomsong.Song = await _context.Songs.FindAsync(roomsong.SongId);
            }
            return room;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Room>> CreateRoom([FromBody]Room room)
        {
            try
            {
                if (room?.RoomSongs?.Any(s => s.Song != null) ?? false)
                {
                    foreach (var roomSong in room.RoomSongs)
                    {
                        var song = await _context.Songs.FirstOrDefaultAsync(s => $"{s.Name} {s.Artist}" == $"{roomSong.Song.Name} {roomSong.Song.Artist}");
                        if (song != null)
                            roomSong.SongId = song.Id;
                    }
                }
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return CreatedAtAction(nameof(GetSingleRoom), new { room.Id }, room);
        }

        [HttpPatch("{roomId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Room>> AddSongToRoom(int roomId, [FromBody]Song song)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(e => e.RoomSongs)
                    .SingleOrDefaultAsync(s => s.Id == roomId);
                var playlist = _context.SpotifyPlaylist.SingleOrDefaultAsync(s => s.RoomId == roomId);
                var matchingSong = _context.Songs.FirstOrDefaultAsync(s => $"{s.Artist}{s.Name}" == $"{song.Artist}{song.Name}");

                if  (room == null)
                    return NotFound();

                var roomSong = new RoomSong()
                {
                    RoomId = roomId,
                    SongId = (await matchingSong)?.Id ?? 0,
                    Song = song,
                };
                if (!room.RoomSongs.Any(s => s.SongId == roomSong.SongId))
                {
                    room.RoomSongs.Add(roomSong);
                    await _context.SaveChangesAsync();
                }

                if (await playlist != null)
                {
                    var playlistTable = await playlist;
                    var service = new SpotifyService(playlistTable.AuthCode);
                    await service.AddSongToPlaylist(playlistTable, song);
                }

                return room;
            }
            catch
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{roomId}/spotify/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Room>> AddSpotifyAuthToRoom(int roomId, string spotifyAuth)
        {
            try
            {
                var room = await _context.Rooms
                    .SingleOrDefaultAsync(s => s.Id == roomId);

                if (room == null)
                    return NotFound();

                if (room.SpotifyPlaylist == null)
                {
                    var service = new SpotifyService(spotifyAuth);
                    var ownerId = await service.GetUserIdAsync();
                    var playlist = await service.CreatePlaylist(room.Name, ownerId);

                    room.SpotifyPlaylist = (SpotifyPlaylist)playlist;
                    await _context.SaveChangesAsync();
                }

                return room;
            }
            catch
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{roomId}/{songId}/{songRating}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomSong>> AddVoteToSong(int roomId, int songId, short songRating)
        {
            if (songRating != -1 && songRating != 1)
                throw new ArgumentException("Invalid vote", nameof(songRating));

            try
            {
                var room = await _context.Rooms
                    .Include(e => e.RoomSongs)
                    .SingleOrDefaultAsync(s => s.Id == roomId);

                if (room == null)
                    return NotFound();

                var roomSong = room.RoomSongs
                    .SingleOrDefault(s => s.SongId == songId);
                roomSong.SongRating += songRating;
                await _context.SaveChangesAsync();

                return roomSong;
            }
            catch
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
