using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PartyPlaylists.MobileAppService.Interfaces;
using PartyPlaylists.MobileAppService.Models.DataModels;

namespace PartyPlaylists.MobileAppService.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Room>>> GetAllRooms()
        {
            return await _roomRepository.GetAll();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Room> Create([FromBody]Room room)
        {
            _roomRepository.Create(room);
            return CreatedAtAction(nameof(GetAllRooms), new { room.Id }, room);
        }
    }
}
