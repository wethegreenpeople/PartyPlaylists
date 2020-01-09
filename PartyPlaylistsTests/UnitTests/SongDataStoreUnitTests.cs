using Microsoft.EntityFrameworkCore;
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
    public class SongDataStoreUnitTests
    {
        private PlaylistContext _playlistContext;

        [TestInitialize]
        public void TestInit()
        {
            var playlistOptions = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("Songs")
                .Options;

            _playlistContext = new PlaylistContext(playlistOptions);
            _playlistContext.Database.EnsureDeleted();
            if (_playlistContext.Database.EnsureCreated())
            {
                _playlistContext.Songs.Add(new Song() { Name = "New song", });
                _playlistContext.SaveChanges();
            }
        }

        [TestMethod]
        public async Task GetItemAsync_GivenSongId_ReturnSong()
        {
            var songDataStore = new SongDataStore(_playlistContext);

            var song = await songDataStore.GetItemAsync("1");

            Assert.IsNotNull(song);
        }
    }
}
