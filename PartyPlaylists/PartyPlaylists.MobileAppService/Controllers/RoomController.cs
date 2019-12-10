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
using Jose;
using Newtonsoft.Json;

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
        public async Task<ActionResult<Room>> CreateRoom(string userToken, [FromBody]Room room)
        {
            if (string.IsNullOrEmpty(userToken))
                throw new ArgumentException();

            var decodedToken = JWT.Decode(userToken);
            var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

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

                room.Owner = token.Name;
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Room>> AddSongToRoom(string userToken, int roomId, [FromBody]Song song)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(e => e.RoomSongs)
                    .SingleOrDefaultAsync(s => s.Id == roomId);
                var playlist = _context.SpotifyPlaylist.SingleOrDefaultAsync(s => s.RoomId == roomId);
                var matchingSong = _context.Songs.FirstOrDefaultAsync(s => $"{s.Artist}{s.Name}" == $"{song.Artist}{song.Name}");
                var decodedToken = JWT.Decode(userToken);
                var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

                if  (room == null)
                    return NotFound();

                var roomSong = new RoomSong()
                {
                    RoomId = roomId,
                    SongId = (await matchingSong)?.Id ?? 0,
                    Song = song,
                    SongAddedBy = token.Name,
                };
                if (await playlist != null)
                {
                    var playlistTable = await playlist;
                    var service = new SpotifyService(playlistTable.AuthCode);
                    if (string.IsNullOrEmpty(song.SpotifyId))
                    {
                        var spotifySong = await service.GetSong(song.Name);
                        await service.AddSongToPlaylist(playlistTable, spotifySong);
                        song.SpotifyId = spotifySong.SpotifyId;
                    }
                    else
                        await service.AddSongToPlaylist(playlistTable, song);
                }
                if (!room.RoomSongs.Any(s => s.SongId == roomSong.SongId))
                {
                    room.RoomSongs.Add(roomSong);
                    await _context.SaveChangesAsync();
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
                    room.IsSpotifyEnabled = true;
                    await _context.SaveChangesAsync();
                }

                return room;
            }
            catch (Exception ex)
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{roomId}/{songId}/{songRating}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomSong>> AddVoteToSong(string userToken, int roomId, int songId, short songRating)
        {
            if (string.IsNullOrEmpty(userToken))
                throw new ArgumentException();
            if (songRating != -1 && songRating != 1)
                throw new ArgumentException("Invalid vote", nameof(songRating));

            try
            {
                var token = await _context.Tokens.SingleOrDefaultAsync(s => s.JWTToken == userToken);
                var room = await _context.Rooms
                    .Include(e => e.RoomSongs)
                    .ThenInclude(e => e.Song)
                    .Include(e => e.RoomSongs)
                    .ThenInclude(e => e.RoomSongTokens)
                    .Include(e => e.SpotifyPlaylist)
                    .SingleOrDefaultAsync(s => s.Id == roomId);

                if (room == null)
                    return NotFound();

                var roomSong = room.RoomSongs.SingleOrDefault(s => s.SongId == songId);

                if (roomSong.RoomSongTokens != null && roomSong.RoomSongTokens.Any(s => s.Token == token))
                    return BadRequest();

                roomSong.SongRating += songRating;

                var roomSongToken = new RoomSongToken()
                {
                    Token = token,
                    TokenId = token.Id,
                };
                roomSong.RoomSongTokens.Add(roomSongToken);

                if (room.IsSpotifyEnabled)
                {
                    var service = new SpotifyService(room.SpotifyPlaylist.AuthCode);
                    await service.ReorderPlaylist(room.SpotifyPlaylist, room);
                }
                await _context.SaveChangesAsync();
                

                return roomSong;
            }
            catch (Exception ex)
            {
                throw new SystemWeb.HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
