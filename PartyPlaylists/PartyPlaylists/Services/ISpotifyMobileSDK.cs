using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Services
{
    public interface ISpotifyMobileSDK
    {
        bool IsAuthenticated { get; }
        void Authenticate();
        void PlaySong(string spotifySongId);
    }
}
