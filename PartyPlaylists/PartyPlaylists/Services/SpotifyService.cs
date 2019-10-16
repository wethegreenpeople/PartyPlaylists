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
using RestSharp;
using RestSharp.Authenticators;

namespace PartyPlaylists.Services
{
    public class SpotifyService : IStreamingService<Song>
    {
        public string AuthToken { get; set; }

        public SpotifyService()
        {

        }

        public SpotifyService(string authCode)
        {
            AuthToken = authCode;
        }

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

        public async Task<string> GetUserId()
        {
            var client = new RestClient(@"https://api.spotify.com");
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(AuthToken);
            var request = new RestRequest(@"v1/me", Method.GET);

            var response = client.Execute(request);
            var content = response.Content;

            return content;
        }
    }
}
