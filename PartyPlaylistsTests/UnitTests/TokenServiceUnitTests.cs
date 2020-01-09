using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyPlaylists.Contexts;
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
            var tokenSerivce = new TokenService(_playlistContext);
            var token = await tokenSerivce.GetToken();

            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
        }
    }
}
