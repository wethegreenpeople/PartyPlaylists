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
using Newtonsoft.Json;
using PartyPlaylists.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using SpotifyApi.NetCore.Authorization;

namespace PartyPlaylists.Services
{
    public class SpotifyService : IStreamingService
    {
        public string AuthToken { get; set; }

        public SpotifyService()
        {

        }

        public SpotifyService(string authCode)
        {
            AuthToken = authCode;
        }

        public async Task AddSongToPlaylist(IPlaylist playlist, Song song)
        {
            try
            {
                var client = new RestClient(@"https://api.spotify.com/v1");
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator($"Bearer {AuthToken}");
                var request = new RestRequest($@"playlists/{playlist.PlaylistID}/tracks", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string[] spotifyUris = { song.SpotifyId };
                request.AddJsonBody(new { uris = spotifyUris });

                var response = await client.ExecuteAsync(request);
                var content = response.Content;
            }
            catch
            {
            }
        }

        public Task Authenticate()
        {
            return null;
        }

        public async Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId, string roomUrl)
        {
            try
            {
                var client = new RestClient(@"https://api.spotify.com/v1");
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator($"Bearer {AuthToken}");
                var request = new RestRequest($@"users/{ownerId}/playlists", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { name = playlistName, description = roomUrl });

                var response = await client.ExecuteAsync(request);
                var content = response.Content;

                dynamic results = JsonConvert.DeserializeObject(content);
                var playlist = new SpotifyPlaylist()
                {
                    AuthCode = AuthToken,
                    PlaylistName = playlistName,
                    PlaylistOwnerID = ownerId,
                    PlaylistID = results.id,
                };

                return playlist;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Song> GetSong(string searchQuery)
        {
            try
            {
                var http = new HttpClient();
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                var accounts = new AccountsService(http, config);

                var search = new SearchApi(http, accounts);
                var searchResult = await search.Search(searchQuery, "track", "");

                var song = new Song()
                {
                    Artist = searchResult?.Tracks?.Items[0].Artists[0].Name,
                    Name = searchResult?.Tracks?.Items[0].Name,
                    ServiceAvailableOn = Enums.StreamingServiceTypes.Spotify,
                    SpotifyId = searchResult?.Tracks.Items[0].Uri,
                    AlbumArt = searchResult?.Tracks.Items[0].Album.Images[0].Url,
                };

                return song;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Song>> GetSongs(string searchQuery)
        {
            try
            {
                var http = new HttpClient();
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                var accounts = new AccountsService(http, config);

                var search = new SearchApi(http, accounts);
                var searchResults = await search.Search(searchQuery, "track", "");

                var songList = new List<Song>();
                foreach (var result in searchResults.Tracks.Items)
                {
                    songList.Add(new Song()
                    {
                        Artist = result.Artists[0].Name,
                        Name = result.Name,
                        ServiceAvailableOn = Enums.StreamingServiceTypes.Spotify,
                        SpotifyId = result.Uri,
                    });
                }

                return songList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetUserIdAsync()
        {
            var client = new RestClient(@"https://api.spotify.com");
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator($"Bearer {AuthToken}");
            var request = new RestRequest(@"v1/me", Method.GET);

            var response = await client.ExecuteAsync(request);
            var content = response.Content;

            dynamic results = JsonConvert.DeserializeObject(content);
            var userId = results.id;

            return userId;
        }
        public async Task ReorderPlaylist(IPlaylist playlist, Room room)
        {
            try
            {
                var client = new RestClient(@"https://api.spotify.com/v1");
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator($"Bearer {AuthToken}");
                var request = new RestRequest($@"playlists/{playlist.PlaylistID}/tracks", Method.PUT);
                request.RequestFormat = DataFormat.Json;

                var spotifyUris = room.RoomSongs
                    .OrderByDescending(s => s.SongRating)
                    .Select(s => s.Song.SpotifyId)
                    .ToList();

                // Move the currently playing song to the top of the playlist
                // to ensure that the next song played is the highest rated
                // regardless of re-orders
                var currentSong = await GetCurrentlyPlayingSongAsync();
                if (currentSong != null)
                {
                    spotifyUris.Remove(currentSong.Item.Uri);
                    spotifyUris.Insert(0, currentSong.Item.Uri);
                }
                
                request.AddJsonBody(new { uris = spotifyUris });

                var response = await client.ExecuteAsync(request);
                var content = response.Content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CurrentPlaybackContext> GetCurrentlyPlayingSongAsync()
        {
            try
            {
                var http = new HttpClient();
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var player = new PlayerApi(http, AuthToken);
                return await player.GetCurrentPlaybackInfo(AuthToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
