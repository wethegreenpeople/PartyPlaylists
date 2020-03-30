using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;

namespace PartyPlaylists.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> JoinRoom(IndexVM viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.RoomToJoin))
                return StatusCode(500);

            return RedirectToAction("Index", "Room", new { Id = viewModel.RoomToJoin });
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
