using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PartyPlaylists.Models.DataModels;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Models;

namespace PartyPlaylists.Services
{
    public class SpotifyService : IStreamingService<Song>
    {
        public Task AddSongToPlaylist(Song song)
        {
            throw new NotImplementedException();
        }

        public Task CreatePlaylist(string playlistName)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> GetSong(string searchQuery)
        {
            var http = new HttpClient();
            var accounts = new AccountsService(http);

            var search = new SearchApi(http, accounts);
            var searchResult = await search.Search(searchQuery, "track", "", (1,0));

            var song = new Song()
            {
                Artist = searchResult.Tracks.Items[0].Artists[0].Name,
                Name = searchResult.Tracks.Items[0].Name,
                ServiceAvailableOn = Enums.StreamingServiceTypes.Spotify,
            };

            return song;
        }
    }
}
