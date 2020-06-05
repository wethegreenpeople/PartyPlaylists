﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public interface ISpotifyMobileSDK
    {
        bool IsAuthenticated { get; }
        Task Authenticate();
        void PlaySong(string spotifySongId);
    }
}
