using Microsoft.EntityFrameworkCore;
using PartyPlaylists.MobileAppService.Contexts;
using PartyPlaylists.MobileAppService.Interfaces;
using PartyPlaylists.MobileAppService.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MobileAppService.Models
{
    public class RoomRepository : IRoomRepository
    {
        private readonly PlaylistContext _playlistContext;

        public RoomRepository(PlaylistContext context)
        {
            _playlistContext = context;
        }

        public async Task Create(Room room)
        {
            await _playlistContext.Rooms.AddAsync(room);
            await _playlistContext.SaveChangesAsync();
        }

        public Room Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Room>> GetAll()
        {
            return await _playlistContext.Rooms.ToListAsync();
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
