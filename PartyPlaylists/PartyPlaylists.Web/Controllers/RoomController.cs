using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PartyPlaylists.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly IDataStore<Room> _roomDataStore;

        public RoomController(IDataStore<Room> roomDataStore)
        {
            _roomDataStore = roomDataStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Home", "Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Join(string id)
        {
            var room = await _roomDataStore.GetItemAsync(id);

            return View("Room", room);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            var createdRoom = await _roomDataStore.AddItemAsync(room);

            return RedirectToAction("Join", "Room", routeValues: new { id = createdRoom.Id.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> AddSong(string id, string songName)
        {
            var song = new Song()
            {
                Name = songName,
            };
            var updatedRoom = await ((RoomDataStore)_roomDataStore).AddSongToRoomAsync(id, song);
            if (updatedRoom == null)
                return NotFound();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSongsInRoom(string id)
        {
            var room = await _roomDataStore.GetItemAsync(id);
            var roomSongs = room.RoomSongs.ToList();

            return PartialView("_SongsInRoom", roomSongs);
        }
    }
}
