using Newtonsoft.Json;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class RoomDataStore : IDataStore<Room>
    {
        private readonly HttpClient client;
        private readonly IEnumerable<Room> _rooms;

        public RoomDataStore()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(@"http://40.117.143.83/partyplaylists/api/")
            };

            _rooms = new List<Room>();
        }

        public async Task<bool> AddItemAsync(Room room)
        {
            if (room == null)
                return false;

            var serializedItem = JsonConvert.SerializeObject(room);
            var response = await client.PostAsync($@"room", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Room> GetItemAsync(string id)
        {
            var json = await client.GetStringAsync($@"room/{id}");
            return await Task.Run(() => JsonConvert.DeserializeObject<Room>(json));
        }

        public Task<IEnumerable<Room>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Room item)
        {
            throw new NotImplementedException();
        }
    }
}
