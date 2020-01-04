using Jose;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PartyPlaylists.MobileAppService.Contexts;
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
            client = new HttpClient()
            {
                BaseAddress = new Uri(@"http://40.117.143.83/partyplaylists/api/")
            };

            _rooms = new List<Room>();
        }

        public async Task<Room> AddItemAsync(Room room)
        {
            return null;
        }

        public async Task<Room> AddItemAsync(Room room, string userToken)
        {
            if (room == null)
                return null;

            var decodedToken = JWT.Decode(userToken);
            var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

            room.Owner = token.Name;
            var serializedItem = JsonConvert.SerializeObject(room);
            var response = await client.PostAsync($@"room?userToken={userToken}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync();
            var respondedRoom = JsonConvert.DeserializeObject<Room>(responseBody);

            return respondedRoom;
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Room> GetItemAsync(string id)
        {
            var room = await _playlistsContext.Rooms
                .Include(e => e.RoomSongs)
                .SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

            if (room == null)
                throw new ArgumentException("Could not find room from given ID");

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
                var playlist = _playlistsContext.SpotifyPlaylist.SingleOrDefaultAsync(s => s.RoomId == roomIdAsInt);
                var matchingSong = _playlistsContext.Songs.FirstOrDefaultAsync(s => $"{s.Artist}{s.Name}" == $"{song.Artist}{song.Name}");
                var decodedToken = JWT.Decode(userToken);
                var token = JsonConvert.DeserializeAnonymousType(decodedToken, new { Name = "" });

                if (room == null)
                    throw new Exception("Could not find room");

                var roomSong = new RoomSong()
                {
                    RoomId = roomIdAsInt,
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
            if (roomId == null || spotifyAuthCode == null)
                return null;

            var patchMethod = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(patchMethod, $@"{client.BaseAddress}room/{roomId}/spotify?spotifyAuth={spotifyAuthCode}");

            var response = await client.SendAsync(request);
            var respondedRoom = JsonConvert.DeserializeObject<Room>(await response.Content.ReadAsStringAsync());

            return respondedRoom;
        }

        public async Task<Room> AddVoteToSong(string userToken, int roomId, int songId, short vote)
        {
            var patchMethod = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(patchMethod, $@"{client.BaseAddress}room/{roomId}/{songId}/{vote}?userToken={userToken}");

            var response = await client.SendAsync(request);
            var respondedRoom = JsonConvert.DeserializeObject<Room>(await response.Content.ReadAsStringAsync());
            return respondedRoom;
        }
    }
}
