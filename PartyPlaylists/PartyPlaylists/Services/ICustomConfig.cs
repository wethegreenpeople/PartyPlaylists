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
        string PartyPlaylistsGeneratedToken { get; }

        Task<ICustomConfig> Build(IFileStorage fileStorage);
        Task SetValue(IFileStorage fileStorage, string propertyToUpdate, string value);
    }
}
