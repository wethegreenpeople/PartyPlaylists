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
        private const string _token = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJpZCI6IjA1ZWVmMmM3LWJmNDctNDM3Zi05NjM0LWNiNjk2NjM3YWU2NCIsIm5hbWUiOiJTdG9ybSBIZWF0aGVyIn0.";

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

        [TestMethod]
        public async Task AddSongToRoomAsync_GivenRoomIDAndSong_ReturnRoomWithSong()
        {
            var roomdatastore = new RoomDataStore(_playlistContext);

            var song = new Song() { Name = "Test Song", Id = 0, Artist = "Swaylo", SpotifyId = "1" };

            Assert.AreEqual(0, (await roomdatastore.GetItemAsync("1")).RoomSongs.Count);

            var room = await roomdatastore.AddSongToRoomAsync(_token, "1", song);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, room.RoomSongs.Count);
            Assert.AreEqual(1, (await roomdatastore.GetItemAsync("1")).RoomSongs.Count);
        }
    }
}
