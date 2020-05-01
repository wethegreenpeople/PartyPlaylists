using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PartyPlaylists.Enums;
using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class AppleMusicService : IStreamingService
    {
        private readonly RestClient _client;

        /// <summary>
        /// Establish an Apple Music Service to wrap the Apple Music API.
        /// More info: https://developer.apple.com/documentation/applemusicapi
        /// </summary>
        /// <param name="config">Expecting 'Keys:AppleMusic:Key,KeyId,TeamId' structure.</param>
        public AppleMusicService(IConfiguration config) : this(
            config["Keys:AppleMusic:Key"], config["Keys:AppleMusic:KeyId"], config["Keys:AppleMusic:TeamId"])
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config),
                    "Need configuration for JWT keys.");
        }
        /// <summary>
        /// Establish an Apple Music Service to wrap the Apple Music API.
        /// More info: https://developer.apple.com/documentation/applemusicapi
        /// </summary>
        /// <param name="key">MusicKit private key.</param>
        /// <param name="keyId">10-character key identifier (kid) key obtained from the Apple developer account.</param>
        /// <param name="teamId">10-character issuer (iss) registered claim key obtained from the Apple developer account.</param>
        public AppleMusicService(string key, string keyId, string teamId) : this(
            CreateSignedJwt(key, keyId, teamId))
        { }
        /// <summary>
        /// Establish an Apple Music Service to wrap the Apple Music API.
        /// More info: https://developer.apple.com/documentation/applemusicapi
        /// </summary>
        /// <param name="jwt">JSON Web Token signed with the MusicKit private key and encrypted using ECDSA with P-256 and SHA-256.</param>
        public AppleMusicService(string jwt)
        {
            var baseUrl = @"https://api.music.apple.com/v1";

            _client = CreateRestClient(baseUrl, jwt);
        }


        /// <summary>
        /// Search for a single song given a song id or takes the first result of the GetSongs method given a term.
        /// </summary>
        /// <param name="searchQuery">A song id or search term.</param>
        /// <returns></returns>
        public async Task<Song> GetSong(string searchQuery)
        {
            if (int.TryParse(searchQuery, out int songId))
            {
                var request = new RestRequest($@"catalog/us/songs/{songId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = await _client.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = JObject.Parse(response.Content);

                    var song = content["data"][0];
                    return new Song()
                    {
                        Id = Convert.ToInt32(song["id"]),
                        Artist = song["attributes"]["artistName"].ToString(),
                        Name = song["attributes"]["name"].ToString(),
                        ServiceAvailableOn = StreamingServiceTypes.AppleMusic,
                        ServiceId = song["attributes"]["url"].ToString(),
                    };
                }
                else throw new Exception($"Response status not OK. {response.StatusDescription}.");
            }
            else
            {
                var songs = await GetSongs(searchQuery);
                return songs.FirstOrDefault();
            }
        }

        public async Task<List<Song>> GetSongs(string searchQuery)
        {
            var request = new RestRequest(@"catalog/us/search", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("types", "songs");
            request.AddParameter("term", searchQuery);

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JObject.Parse(response.Content);

                var songs = new List<Song>();
                foreach (var song in content["results"]["songs"]["data"])
                {
                    songs.Add(new Song()
                    {
                        Id = Convert.ToInt32(song["id"]),
                        Artist = song["attributes"]["artistName"].ToString(),
                        Name = song["attributes"]["name"].ToString(),
                        ServiceAvailableOn = StreamingServiceTypes.AppleMusic,
                        ServiceId = song["attributes"]["url"].ToString(),
                    });
                }
                return songs;
            }
            else throw new Exception($"Response status not OK. {response.StatusDescription}.");
        }

        public async Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId, string roomUrl)
        {
            // TODO check for "Music-User-Token" in client header (basically ensure Authenticate was called.

            var request = new RestRequest(@"me/library/playlists", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(new { attributes = new { name = playlistName, description = roomUrl } });

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = JObject.Parse(response.Content);
                // TODO create and populate a new AppleMusicPlaylist object
                throw new NotImplementedException();
            }
            else throw new Exception($"Response status not Created. {response.StatusDescription}.");
        }

        public Task AddSongToPlaylist(IPlaylist playlist, Song song)
        {
            throw new NotImplementedException();
        }

        public Task ReorderPlaylist(IPlaylist playlist, Room room)
        {
            throw new NotImplementedException();
        }

        public Task Authenticate()
        {
            // TODO how do we source this Music User Token?
            // Seems like MusicKit JS has an authorize method,
            // but I do not see how to get the token from it,
            // let alone into this method given it is parameterless.
            _client.AddDefaultHeader("Music-User-Token", "");
            return null;
        }

        /// <summary>
        /// Create a signed developer JWT for Apple Music API requests.
        /// More info: https://developer.apple.com/documentation/applemusicapi/getting_keys_and_creating_tokens
        /// </summary>
        /// <param name="key">MusicKit private key.</param>
        /// <param name="keyId">10-character key identifier (kid) key obtained from the Apple developer account.</param>
        /// <param name="teamId">10-character issuer (iss) registered claim key obtained from the Apple developer account.</param>
        /// <returns>JSON Web Token signed with the MusicKit private key and encrypted using ECDSA with P-256 and SHA-256.</returns>
        public static string CreateSignedJwt(string key, string keyId, string teamId)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException(nameof(key),
                    "Must not be null, empty, or consist only of white-space characters.");
            else if (string.IsNullOrWhiteSpace(keyId))
                throw new ArgumentException(nameof(keyId),
                    "Must not be null, empty, or consist only of white-space characters.");
            else if (string.IsNullOrWhiteSpace(teamId))
                throw new ArgumentException(nameof(teamId),
                    "Must not be null, empty, or consist only of white-space characters.");

            var securityKey = new ECDsaSecurityKey(
                new ECDsaCng(
                    CngKey.Import(
                        Convert.FromBase64String(key),
                        CngKeyBlobFormat.Pkcs8PrivateBlob)))
            { KeyId = keyId };

            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.CreateJwtSecurityToken(
                issuer: teamId,
                notBefore: now,
                expires: now.AddMinutes(3), // TODO determine best expiration
                issuedAt: now,
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.EcdsaSha256));

            return tokenHandler.WriteToken(jwtToken);
        }

        /// <summary>
        /// Create a RestClient with attached Authenticator (using a JSON Web Token).
        /// </summary>
        /// <param name="baseUrl">Base URL for requests made by this client.</param>
        /// <param name="jwt">JSON Web Token signed with the MusicKit private key and encrypted using ECDSA with P-256 and SHA-256.</param>
        /// <returns>RestSharp.RestClient</returns>
        public static RestClient CreateRestClient(string baseUrl, string jwt = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException(nameof(baseUrl),
                    "Must not be null, empty, or consist only of white-space characters.");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return new RestClient(baseUrl);
            }
            else return new RestClient(baseUrl)
            {
                Authenticator = new JwtAuthenticator(jwt)
            };
        }
    }
}
