using Microsoft.EntityFrameworkCore;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using SpotifyApi.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class SpotifyPlaylistsStore : IDataStore<SpotifyPlaylist>
    {
        private readonly PlaylistContext _playlistContext;

        public SpotifyPlaylistsStore(PlaylistContext context)
        {
            _playlistContext = context;
        }

        public async Task<SpotifyPlaylist> AddItemAsync(SpotifyPlaylist item)
        {
            try
            {
                await _playlistContext.SpotifyPlaylist.AddAsync(item);
                await _playlistContext.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<SpotifyPlaylist> GetItemAsync(string id) => 
            await _playlistContext.SpotifyPlaylist.SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

        public Task<IEnumerable<SpotifyPlaylist>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<SpotifyPlaylist> UpdateItemAsync(SpotifyPlaylist item)
        {
            throw new NotImplementedException();
        }
    }
}
