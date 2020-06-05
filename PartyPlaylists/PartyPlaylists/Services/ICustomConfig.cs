using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface ICustomConfig
    {
        string PartyPlaylistsKey { get; }
        string SpotifyClientId { get; }

        Task<ICustomConfig> Build(IFileStorage fileStorage);
    }
}
