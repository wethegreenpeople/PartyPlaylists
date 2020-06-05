using Microsoft.AspNetCore.SignalR;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using SpotifyApi.NetCore;
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

        public async Task PlayNextSongAsync(string roomId)
        {
            await Clients.All.SendAsync("PlayNextSong", roomId);
        }
    }
}
