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
        private PlaylistContext playlistContext;
        RoomSongDataStore roomSongDataStore;

        [TestInitialize]
        public void TestInit()
        {
            DbContextOptionsBuilder<PlaylistContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("RoomSong");
            playlistContext = new PlaylistContext(dbContextOptionsBuilder.Options);
            roomSongDataStore = new RoomSongDataStore(playlistContext);

            playlistContext.Rooms.Add(
               new Room()
               {
                   Id = 1,
                   Owner = "Owner",

                   RoomSongs = new List<RoomSong>()
                   {
                        new RoomSong()
                        {
                           Id = 1,
                           RoomId = 1,
                        }
                   }
               });
        }

        [TestMethod]
        public async Task DeleteItemAsync_GivenRoomSongId_ReturnBool()
        {
            Assert.IsNotNull(playlistContext.RoomSongs.Where(i => i.Id == 1));
            Assert.IsTrue(await roomSongDataStore.DeleteItemAsync("1"));
            Assert.IsNull(await playlistContext.RoomSongs.FindAsync(1));
        }
        [TestMethod]
        public async Task GetItemAsync_GivenRoomSongId_ReturnRoomSong()
        {
            RoomSong mockRoomSong = new RoomSong() { Id = 2 };
            playlistContext.RoomSongs.Add(mockRoomSong);
            Assert.IsNotNull(await playlistContext.RoomSongs.FindAsync(1));
            Assert.AreEqual(mockRoomSong, await roomSongDataStore.GetItemAsync("2"));
        }
        [TestMethod]
        public async Task GetItemAsync_GivenRoomSongId_ReturnException()
        {
            int checkedValue = 0;
            Assert.IsNull(playlistContext.RoomSongs.Find(checkedValue));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomSongDataStore.GetItemAsync(checkedValue.ToString()));
        }
    }
}
