using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using PartyPlaylists.Web.Models;

namespace PartyPlaylists.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataStore<Room> _roomDataStore;

        public HomeController(IDataStore<Room> roomDataStore)
        {
            _roomDataStore = roomDataStore;
        }

        public IActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JoinRoom(string roomId)
        {
            var room = await _roomDataStore.GetItemAsync(roomId);

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(Room room)
        {
            await _roomDataStore.AddItemAsync(room);

            return View("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
