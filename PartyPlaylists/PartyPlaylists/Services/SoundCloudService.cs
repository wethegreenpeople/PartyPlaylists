using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class SoundCloudService : IStreamingService
    {
        public Task AddSongToPlaylist(IPlaylist playlist, Song song)
        {
            throw new NotImplementedException();
        }

        public Task Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IPlaylist> CreatePlaylist(string playlistName, string ownerId, string roomUrl)
        {
            throw new NotImplementedException();
        }

        public Task<Song> GetSong(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<List<Song>> GetSongs(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task ReorderPlaylist(IPlaylist playlist, Room room)
        {
            throw new NotImplementedException();
        }
    }
}
