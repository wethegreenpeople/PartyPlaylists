using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface IStreamingService
    {
        Task<Song> GetSong(string searchQuery);
        Task<List<Song>> GetSongs(string searchQuery);
        Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId);
        Task AddSongToPlaylist(IPlaylist playlist, Song song);
        Task Authenticate();
        Task ReorderPlaylist(IPlaylist playlist, Room room, RoomSong latestVotedSong = null);
    }
}
