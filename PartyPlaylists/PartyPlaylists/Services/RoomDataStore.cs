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

        public async Task<Room> AddItemAsync(Room room)
        {
            if (room == null)
                return null;

            var serializedItem = JsonConvert.SerializeObject(room);
            var response = await client.PostAsync($@"room", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
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
            var json = await client.GetStringAsync($@"room/{id}");
            return await Task.Run(() => JsonConvert.DeserializeObject<Room>(json));
        }

        public Task<IEnumerable<Room>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<Room> UpdateItemAsync(Room room)
        {
            return null;
        }

        public async Task<Room> AddSongToRoomAsync(string roomId, Song song)
        {
            if (roomId == null || song == null)
                return null;

            var patchMethod = new HttpMethod("PATCH");
            var serializedItem = JsonConvert.SerializeObject(song);
            var content = new StringContent(serializedItem, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(patchMethod, $@"{client.BaseAddress}room/{roomId}")
            {
                Content = content,
            };

            var response = await client.SendAsync(request);
            var respondedRoom = JsonConvert.DeserializeObject<Room>(await response.Content.ReadAsStringAsync());

            return respondedRoom;
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

        public async Task<Room> AddVoteToSong(int roomId, int songId, short vote)
        {
            var patchMethod = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(patchMethod, $@"{client.BaseAddress}room/{roomId}/{songId}/{vote}");

            var response = await client.SendAsync(request);
            var respondedRoom = JsonConvert.DeserializeObject<Room>(await response.Content.ReadAsStringAsync());
            return respondedRoom;
        }
    }
}
