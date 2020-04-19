using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class AppleMusicService : IStreamingService
    {
        public Task AddSongToPlaylist(IPlaylist playlist, Song song)
        {
            throw new NotImplementedException();
        }

        public Task Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId, string roomUrl)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> GetSong(string searchQuery)
        {
            var client = new RestClient(@"https://api.music.apple.com/v1");
            var request = new RestRequest($@"catalog/us/search", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("term", searchQuery);

            var response = await client.ExecuteAsync(request);
            var content = response.Content;

            return null;
        }

        public Task<List<Song>> GetSongs(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task ReorderPlaylist(IPlaylist playlist, Room room)
        {
            throw new NotImplementedException();
        }
    }
}
