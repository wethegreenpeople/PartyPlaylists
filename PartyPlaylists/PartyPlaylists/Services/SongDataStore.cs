﻿using Microsoft.EntityFrameworkCore;
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
    public class SongDataStore : IDataStore<Song>
    {
        private readonly PlaylistContext _playlistContext;

        public SongDataStore(PlaylistContext playlistContext)
        {
            _playlistContext = playlistContext;
        }

        public Task<Song> AddItemAsync(Song song)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> GetItemAsync(string id)
        {
            var song = await _playlistContext.Songs.SingleOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

            if (song == null)
                throw new Exception("Could not find song");
            return song;
        }

        // Search for a song by the ID that the service provides 
        public async Task<Song> GetItemByServiceId(string id)
        {
            var song = await _playlistContext.Songs.SingleOrDefaultAsync(s => s.ServiceId == id);
            return song;
        }

        public Task<IEnumerable<Song>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> UpdateItemAsync(Song song)
        {
            throw new NotImplementedException();
        }
    }
}
