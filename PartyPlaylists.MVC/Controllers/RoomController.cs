using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Models.ViewModels;
using PartyPlaylists.Services;

namespace PartyPlaylists.MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly IDataStore<Room> _roomDataStore;
        private readonly TokenService _tokenService;

        public RoomController(IDataStore<Room> roomDataStore, TokenService tokenService)
        {
            _roomDataStore = roomDataStore;
            _tokenService = tokenService;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var roomVm = new RoomVM()
            {
                CurrentRoom = await _roomDataStore.GetItemAsync(Id),
                JwtToken = await _tokenService.GetToken(),
            };

            return View("Index", roomVm);
        }

        [Authorize]
        public async Task<IActionResult> AddVoteToSong(int roomId, int songId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var room = await ((RoomDataStore)_roomDataStore).AddVoteToSong(token, roomId, songId, 1);

            return Ok();
        }
    }
}