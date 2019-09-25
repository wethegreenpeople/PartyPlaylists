using PartyPlaylists.MobileAppService.Models.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyPlaylists.MobileAppService.Interfaces
{
    public interface IRoomRepository
    {
        Task Create(Room room);
        void Update(Room room);
        void Remove(string id);
        Room Get(string id);
        Task<List<Room>> GetAll();
    }
}
