using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylistsTests.UnitTests
{
    [TestClass]
    public class TokenServiceUnitTests
    {
        private PlaylistContext _playlistContext;
        private readonly IConfiguration _config = new ConfigurationBuilder().Build();

        [TestInitialize]
        public void TestInit()
        {
            var playlistOptions = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("Tokens")
                .Options;

            _playlistContext = new PlaylistContext(playlistOptions);
        }

        [TestMethod]
        public async Task GetToken_ReturnTokenString()
        {
            var token = await TokenService.CreateTokenAsync(_config);

            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
        }

        [TestMethod]
        public async Task ValidateToken_GivenValidToken_ReturnTrue()
        {
            var key = Guid.NewGuid().ToString();
            var tokenService = new TokenService(null, _config);
            var token = await TokenService.CreateTokenAsync(key);
            Assert.IsTrue(tokenService.ValidateToken(token, key));
        }

        [TestMethod]
        public void ValidateToken_GivenInvalidToken_ReturnFalse()
        {
            var key = Guid.NewGuid().ToString();
            var tokenService = new TokenService(null, _config);
            Assert.IsFalse(tokenService.ValidateToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJRdWlsdCBQeXJpdGUiLCJuYmYiOjE1ODc1Mzc2ODcsImV4cCI6MTU4NzYyNDA4NywiaWF0IjoxNTg3NTM3Njg3fQ.j-aT8ZTCDXdRJswu3267Ar5OtHVRjR7w1EBivDNEQYA", key));
        }
    }
}
