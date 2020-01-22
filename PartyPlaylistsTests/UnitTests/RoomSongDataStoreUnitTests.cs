using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylistsTests.UnitTests
{
    [TestClass]
    public class RoomSongDataStoreUnitTests
    {
        private PlaylistContext _playlistContext;
        private RoomSongDataStore _roomSongDataStore;
        private RoomSong _mockRoomSong;
        [TestInitialize]
        public void TestInit()
        {
            DbContextOptionsBuilder<PlaylistContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("RoomSong");

            Room mockRoom = new Room()
            {
                Id = 1,
                Owner = "Owner",
            };
            Song mockSong = new Song()
            {
                Id = 1,
                Name = "Song1",
                Artist = "Artist1",
            };
            _mockRoomSong = new RoomSong()
            {
                Id = 1,
                RoomId = 1,
                Room = mockRoom,
                Song = mockSong,
                SongId = 1,
            };

            _playlistContext = new PlaylistContext(dbContextOptionsBuilder.Options);
            _playlistContext.Database.EnsureDeleted();
            _playlistContext.Rooms.Add(mockRoom);
            _playlistContext.RoomSongs.Add(_mockRoomSong);
            _playlistContext.Songs.Add(mockSong);
            _playlistContext.SaveChanges();
            _roomSongDataStore = new RoomSongDataStore(_playlistContext);
        }

        [TestMethod]
        public async Task DeleteItemAsync_GivenRoomSongId_ReturnBool()
        {
            int checkedValue = 1;
            var a = _playlistContext;
            Assert.IsNotNull(await _roomSongDataStore.GetItemAsync(checkedValue.ToString()));
            Assert.IsTrue(await _roomSongDataStore.DeleteItemAsync(checkedValue.ToString()));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await   _roomSongDataStore.GetItemAsync(checkedValue.ToString()));
        }
        [TestMethod]
        public async Task GetItemAsync_GivenRoomSongId_ReturnRoomSong()
        {
            int checkedValue = 2;
            RoomSong mockRoomSong = new RoomSong() { Id = checkedValue };
            Assert.IsNotNull(await _roomSongDataStore.GetItemAsync(checkedValue.ToString()));
            Assert.AreEqual(mockRoomSong, await _roomSongDataStore.GetItemAsync(checkedValue.ToString()));
        }
        [TestMethod]
        public async Task GetItemAsync_GivenRoomSongId_ReturnException()
        {
            int[] checkedValues = { -99, 0, 1000};
            foreach(int value in checkedValues)
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _roomSongDataStore.GetItemAsync(value.ToString()));
            } 
        }
    }
}
