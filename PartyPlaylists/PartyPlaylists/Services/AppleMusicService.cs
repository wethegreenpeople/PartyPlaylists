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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class AppleMusicService : IStreamingService
    {
        private static readonly string _baseUrl = @"https://api.music.apple.com/v1";

        private readonly RestClient _client;


        public AppleMusicService()
        {
            _client = new RestClient(_baseUrl)
            {
                // TODO source the key, keyId, and teamId
                Authenticator = new JwtAuthenticator(CreateSignedJwt("", "", ""))
            };
        }


        public Task<Song> GetSong(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<List<Song>> GetSongs(string searchQuery)
        {
            throw new NotImplementedException();

            // TODO try once JWT keys have been sourced for client construction
            //var request = new RestRequest(@"catalog/us/search", Method.GET)
            //{
            //    RequestFormat = DataFormat.Json
            //};
            //request.AddParameter("types", "songs");
            //request.AddParameter("term", searchQuery);

            //var response = await _client.ExecuteAsync(request);
            //var content = JObject.Parse(response.Content);

            //var songs = new List<Song>();
            //foreach (var song in content["results"]["songs"]["data"])
            //{
            //    songs.Add(new Song()
            //    {
            //        Artist = song["attributes"]["artistName"].ToString(),
            //        Name = song["attributes"]["name"].ToString(),
            //        ServiceAvailableOn = StreamingServiceTypes.AppleMusic,
            //        ServiceId = song["attributes"]["url"].ToString(),
            //    });
            //}
            //return songs;
        }

        public Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId, string roomUrl)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a signed developer JWT for Apple Music API requests.
        /// More info: https://developer.apple.com/documentation/applemusicapi/getting_keys_and_creating_tokens
        /// </summary>
        /// <param name="key">MusicKit private key.</param>
        /// <param name="keyId">10-character key identifier (kid) key obtained from the Apple developer account.</param>
        /// <param name="teamId">10-character issuer (iss) registered claim key obtained from the Apple developer account.</param>
        /// <returns>JSON Web Token signed with the MusicKit private key and encrypted using ECDSA with P-256 and SHA-256.</returns>
        private static string CreateSignedJwt(string key, string keyId, string teamId)
        {
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
    }
}
