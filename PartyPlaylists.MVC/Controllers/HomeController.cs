using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;
using PartyPlaylists.Contexts;
using SpotifyApi.NetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PartyPlaylists.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlaylistContext _playlistContext;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, PlaylistContext playlistContext, IConfiguration config)
        {
            _logger = logger;
            _playlistContext = playlistContext;
            _tokenService = new TokenService(playlistContext, config);
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult JoinRoom(IndexVM viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.RoomToJoin))
                return StatusCode(500);

            return RedirectToAction("Index", "Room", new { Id = viewModel.RoomToJoin });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(IndexVM viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.NewRoomName))
                return StatusCode(500);

            Request.Cookies.TryGetValue("jwtToken", out string jwtToken);
            if (string.IsNullOrEmpty(jwtToken))
            {
                var option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                jwtToken = await TokenService.CreateTokenAsync(_config);
                await _tokenService.SaveTokenAsync(jwtToken);
                Response.Cookies.Append("jwtToken", jwtToken, option);
            }

            var ownerName = _tokenService.GetNameFromToken(jwtToken);
            var room = new Room()
            {
                Name = viewModel.NewRoomName,
                IsSpotifyEnabled = false,
                Owner = ownerName,
            };
            var createdRoom = await new RoomDataStore(_playlistContext).AddItemAsync(room, jwtToken);
            return RedirectToAction("Index", "Room", new { Id = createdRoom.Id });
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
