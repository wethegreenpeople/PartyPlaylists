using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MVC.Hubs
{
    public class RoomHub : Hub
    {
        public async Task UpdateSongsAsync(string roomId)
        {
            await Clients.All.SendAsync("Update", roomId);
        }
    }
}
