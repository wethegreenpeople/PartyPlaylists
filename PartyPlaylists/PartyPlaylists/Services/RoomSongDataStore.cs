using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace PartyPlaylists.Services
{
    public class RoomSongDataStore : IDataStore<RoomSong>
    {
        private readonly HttpClient client;
        private readonly PlaylistContext playlistContext;

        public RoomSongDataStore(PlaylistContext playlistContext)
        {
            this.playlistContext = playlistContext;
        }

        public Task<RoomSong> AddItemAsync(RoomSong item)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var roomSong =  GetItemAsync(id);
            
            playlistContext.RoomSongs.Remove(await roomSong);
            await playlistContext.SaveChangesAsync();
            return true;
        }

        public async Task<RoomSong> GetItemAsync(string id)
        {
            var roomSong = await playlistContext.RoomSongs.FindAsync(Convert.ToInt32(id));
            if (roomSong == null)
                throw new ArgumentException($"Could not find RoomSong given ID:{id}");
            return roomSong;
        }

        public Task<IEnumerable<RoomSong>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<RoomSong> UpdateItemAsync(RoomDataStore item)
        {
            throw new NotImplementedException();
        }

        public Task<RoomSong> UpdateItemAsync(RoomSong item)
        {
            throw new NotImplementedException();
        }
    }
}
