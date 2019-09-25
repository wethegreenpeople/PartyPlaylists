using PartyPlaylists.MobileAppService.Models.DataModels;
using System.Collections.Generic;

namespace PartyPlaylists.MobileAppService.Interfaces
{
    public interface IRoomRepository
    {
        void Create(Room room);
        void Update(Room room);
        void Remove(string id);
        Room Get(string id);
        IEnumerable<Room> GetAll();
    }
}
