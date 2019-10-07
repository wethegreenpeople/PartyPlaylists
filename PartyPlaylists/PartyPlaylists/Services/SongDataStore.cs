using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class SongDataStore : IDataStore<Song>
    {
        private readonly HttpClient client;
        private readonly IEnumerable<Song> _songs;

        public SongDataStore()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(@"http://40.117.143.83/partyplaylists/api/")
            };

            _songs = new List<Song>();
        }

        public Task<Song> AddItemAsync(Song song)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Song> GetItemAsync(string id)
        {
            throw new NotImplementedException();
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
