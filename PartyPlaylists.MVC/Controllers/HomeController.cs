using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PartyPlaylists.MVC.Models;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.MVC.DataAccess;
namespace PartyPlaylists.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        PlaylistContext _playlistContext;
        public HomeController(PlaylistContext playlistContext, ILogger<HomeController> logger)
        {
            _playlistContext = playlistContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
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
