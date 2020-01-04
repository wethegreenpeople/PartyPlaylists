using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyPlaylists.MobileAppService.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylistsTests.UnitTests
{
    [TestClass]
    public class RoomDataStoreUnitTests
    {
        private PlaylistContext _playlistContext;

        [TestInitialize]
        public void TestInit()
        {
            var playlistOptions = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("Rooms")
                .Options;

            _playlistContext = new PlaylistContext(playlistOptions);

            _playlistContext.Add(new Room() { Id = 1, Name = "Room one", });
            _playlistContext.Add(new Room() { Id = 2, Name = "Room Two", });
            _playlistContext.Add(new Room() { Id = 3, Name = "Room Three", });
            _playlistContext.SaveChanges();
        }

        [TestMethod]
        public async Task GetItemAsync_GivenRoomId_ReturnRoom()
        {
            var roomDataStore = new RoomDataStore(_playlistContext);

            var room = await roomDataStore.GetItemAsync("1");

            Assert.IsNotNull(room);
        }


        [TestMethod]
        public async Task GetItemAsync_GivenRoomId_ReturnException()
        {
            var roomDataStore = new RoomDataStore(_playlistContext);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomDataStore.GetItemAsync("0"));
        }
    }
}
