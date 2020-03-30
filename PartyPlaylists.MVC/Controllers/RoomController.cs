using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly IDataStore<Room> _roomDataStore;

        public RoomController(IDataStore<Room> roomDataStore)
        {
            _roomDataStore = roomDataStore;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var roomVm = new RoomVM()
            {
                CurrentRoom = await _roomDataStore.GetItemAsync(Id),
            };

            return View("Index", roomVm);
        }
    }
}