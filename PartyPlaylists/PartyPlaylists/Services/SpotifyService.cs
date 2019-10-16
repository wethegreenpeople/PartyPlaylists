using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PartyPlaylists.Models.DataModels;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Models;
using System.Web;
using System.Net.Http.Headers;

namespace PartyPlaylists.Services
{
    public class SpotifyService : IStreamingService<Song>
    {
        public Task AddSongToPlaylist(Song song)
        {
            throw new NotImplementedException();
        }

        public Task Authenticate()
        {
            return null;
        }

        public Task CreatePlaylist(string playlistName)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> GetSong(string searchQuery)
        {
            try
            {
                var http = new HttpClient();
                var accounts = new AccountsService(http);

                var search = new SearchApi(http, accounts);
                var searchResult = await search.Search(searchQuery, "track", "", (1, 0));

                var song = new Song()
                {
                    Artist = searchResult?.Tracks?.Items[0].Artists[0].Name,
                    Name = searchResult?.Tracks?.Items[0].Name,
                    ServiceAvailableOn = Enums.StreamingServiceTypes.Spotify,
                };

                return song;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetUserId(string token)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer {token}");
            var result = await http.GetStringAsync(@"https://api.spotify.com/v1/me");

            return result;
        }
    }
}
