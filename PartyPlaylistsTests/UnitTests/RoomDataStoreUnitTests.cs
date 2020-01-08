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
        private static Song[] _songs = { new Song() { Artist = "Artist One", Id = 1, Name = "Song One" }, new Song() { Artist = "Artist Two", Id = 2, Name = "Song Two"} };
        private RoomSong[] roomSong = {
            new RoomSong()
        {
            RoomId = 1,
            SongId = 1,
            Song = _songs[0]
        },  
            new RoomSong()
        {
            RoomId = 1,
            SongId = 2,
            Song = _songs[1]
        }

        };

        [TestInitialize]
        public void TestInit()
        {
            var playlistOptions = new DbContextOptionsBuilder<PlaylistContext>()
                .UseInMemoryDatabase("Rooms")
                .Options;

            _playlistContext = new PlaylistContext(playlistOptions);
            if (_playlistContext.Database.EnsureCreated())
            {
                _playlistContext.Add(new Token() { JWTToken = _token, });
                _playlistContext.Add(new Room() { Id = 1, Name = "Room one", RoomSongs = new List<RoomSong>(roomSong) });
                _playlistContext.Add(new Room() { Id = 2, Name = "Room Two", });
                _playlistContext.Add(new Room() { Id = 3, Name = "Room Three", });
                _playlistContext.SaveChanges();
            }
        }

        [TestMethod]
        public async Task GetItemAsync_GivenRoomId_ReturnRoom()
        {
            var roomDataStore = new RoomDataStore(_playlistContext);

            var room = await roomDataStore.GetItemAsync("1");

            Assert.IsNotNull(room);
        }


        [TestMethod]
        public async Task GetItemAsync_GivenInvalidRoomId_ReturnException()
        {
            var roomDataStore = new RoomDataStore(_playlistContext);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomDataStore.GetItemAsync("0"));
        }

        [TestMethod]
        public async Task AddSongToRoomAsync_GivenRoomIDAndSong_ReturnRoomWithSong()
        {
            var roomdatastore = new RoomDataStore(_playlistContext);

            Assert.AreEqual(0, (await roomdatastore.GetItemAsync("2")).RoomSongs.Count);

            var room = await roomdatastore.AddSongToRoomAsync(_token, "2", _songs[0]);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, room.RoomSongs.Count);
            Assert.AreEqual(1, (await roomdatastore.GetItemAsync("2")).RoomSongs.Count);
        }

        [TestMethod]
        public async Task AddVoteToSong_GivenRoomIDSongIDAndVote_ReturnRoomwithSong()
        {

            var roomdatastore = new RoomDataStore(_playlistContext);

            Assert.AreEqual(0, (await roomdatastore.GetItemAsync("1")).RoomSongs[1].SongRating);

            var room = await roomdatastore.AddVoteToSong(_token, 1, 1, 1);

            Assert.IsNotNull(room);
            Assert.AreEqual(1, room.RoomSongs[0].SongRating);
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomdatastore.AddVoteToSong(_token, 1, 1, 2));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomdatastore.AddVoteToSong(_token, 1, 1, -2));
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await roomdatastore.AddVoteToSong(null, 1, 1, 1));
        }
    }
}
