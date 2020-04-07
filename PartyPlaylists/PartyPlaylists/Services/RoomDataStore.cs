using Jose;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class RoomDataStore : IDataStore<Room>
    {
        private readonly HttpClient client;
        private readonly IEnumerable<Room> _rooms;
        private readonly PlaylistContext _playlistsContext;

        public RoomDataStore(PlaylistContext playlistsContext)
        {
            _playlistsContext = playlistsContext;
            _rooms = new List<Room>();
        }

        public async Task<Room> AddItemAsync(Room room, string userToken)
        {
            if (string.IsNullOrEmpty(userToken))
                throw new ArgumentException();

            var decodedToken = JWT.Decode(userToken);
            var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

            try
            {
                // If we're given a room with songs, we need to make sure
                // to populate the room table with the correct song data
                if (room?.RoomSongs?.Any(s => s.Song != null) ?? false)
                {
                    foreach (var roomSong in room.RoomSongs)
                    {
                        var song = await _playlistsContext.Songs.FirstOrDefaultAsync(s => $"{s.Name} {s.Artist}" == $"{roomSong.Song.Name} {roomSong.Song.Artist}");
                        if (song != null)
                            roomSong.SongId = song.Id;
                    }
                }

                room.Owner = token.Name;
                _playlistsContext.Rooms.Add(room);
                await _playlistsContext.SaveChangesAsync();

                return room;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var room = await GetItemAsync(id);

            if (room == null)
                return false;
            
            _playlistsContext.Remove(room);
            await _playlistsContext.SaveChangesAsync();
            return true;
        }

        public async Task<Room> GetItemAsync(string id)
        {

            var room = await _playlistsContext.Rooms
                .Include(e => e.RoomSongs)
                .SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(id));


            if (room == null)
                throw new ArgumentException($"Could not find Room from given ID:{id}");

            foreach (var roomsong in room.RoomSongs)
            {
                roomsong.Song = await _playlistsContext.Songs.FindAsync(roomsong.SongId);
            }

            return room;
        }

        public Task<IEnumerable<Room>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<Room> UpdateItemAsync(Room room)
        {
            return null;
        }

        public async Task<Room> AddSongToRoomAsync(string userToken, string roomId, Song song)
        {
            try
            {
                var roomIdAsInt = Convert.ToInt32(roomId);
                var room = await GetItemAsync(roomId);
                var playlist = await _playlistsContext.SpotifyPlaylist.SingleOrDefaultAsync(s => s.RoomId == roomIdAsInt);
                var matchingSong = await _playlistsContext.Songs.FirstOrDefaultAsync(s => s.SpotifyId == song.SpotifyId);
                var decodedToken = JWT.Decode(userToken);
                var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

                if (room == null)
                    throw new Exception("Could not find room");

                var roomSong = new RoomSong()
                {
                    RoomId = roomIdAsInt,
                    SongId = (matchingSong)?.Id ?? 0,
                    Song = song,
                    SongAddedBy = token.Name,
                };

                if (playlist != null)
                {
                    var playlistTable = playlist;
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
                    await _playlistsContext.SaveChangesAsync();
                }

                return room;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Room> AddSpotifyAuthCodeToRoomAsync(string roomId, string spotifyAuthCode)
        {
            try
            {
                var room = await _playlistsContext.Rooms
                    .SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(roomId));

                if (room == null)
                    throw new Exception("Room not found");

                if (room.SpotifyPlaylist == null)
                {
                    var service = new SpotifyService(spotifyAuthCode);
                    var ownerId = await service.GetUserIdAsync();
                    var playlist = await service.CreatePlaylist(room.Name, ownerId);

                    room.SpotifyPlaylist = (SpotifyPlaylist)playlist;
                    room.IsSpotifyEnabled = true;
                    await _playlistsContext.SaveChangesAsync();
                }

                return room;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Room> AddVoteToSong(string userToken, int roomId, int songId, short vote)
        {

            if (string.IsNullOrEmpty(userToken))
                throw new ArgumentException();
            if (vote != -1 && vote != 1)
                throw new ArgumentException("Invalid vote", nameof(vote));

            try
            {
                var token = await _playlistsContext.Tokens.SingleOrDefaultAsync(s => s.JWTToken == userToken);
                var room = await _playlistsContext.Rooms
                    .Include(e => e.RoomSongs)
                    .ThenInclude(e => e.Song)
                    .Include(e => e.RoomSongs)
                    .ThenInclude(e => e.RoomSongTokens)
                    .Include(e => e.SpotifyPlaylist)
                    .SingleOrDefaultAsync(s => s.Id == roomId);

                if (room == null)
                    throw new Exception("Could not find room");

                var roomSong = room.RoomSongs.SingleOrDefault(s => s.SongId == songId);

                if (roomSong.RoomSongTokens != null && roomSong.RoomSongTokens.Any(s => s.Token == token))
                    throw new Exception("Couldn't find token in RoomSongTokens");

                roomSong.SongRating += vote;

                var roomSongToken = new RoomSongToken()
                {
                    Token = token,
                    TokenId = token.Id,
                };
                roomSong.RoomSongTokens.Add(roomSongToken);

                await _playlistsContext.SaveChangesAsync();

                if (room.IsSpotifyEnabled)
                {
                    var service = new SpotifyService(room.SpotifyPlaylist.AuthCode);
                    await service.ReorderPlaylist(room.SpotifyPlaylist, room, roomSong);
                }

                return roomSong.Room;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Task<Room> AddItemAsync(Room item)
        {
            throw new NotImplementedException();
        }
    }
}
