using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface IStreamingService<T>
    {
        Task<T> GetSong(string searchQuery);
        Task CreatePlaylist(string playlistName);
        Task AddSongToPlaylist(Song song);

    }
}
