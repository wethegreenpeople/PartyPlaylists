using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Spotify.Android.Appremote.Api;
using Com.Spotify.Protocol.Types;
using Com.Spotify.Protocol.Client;
using Com.Spotify.Android.Appremote.Internal;
using PartyPlaylists.Services;
using Microsoft.AspNetCore.Connections;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Splat;
using PartyPlaylists.Models.DataModels;
using static Com.Spotify.Protocol.Client.Subscription;

namespace PartyPlaylists.Droid
{
    public class SpotifyMobileSDK : ISpotifyMobileSDK
    {
        private readonly Connector _connector;
        private readonly Task<ICustomConfig> _config;

        public bool IsAuthenticated => _connector.SpotifyApp != null;
        public SpotifyMobileSDK()
        {
            _connector = new Connector();
            var fileStorage = Locator.Current.GetService<IFileStorage>();
            _config = Locator.Current.GetService<ICustomConfig>().Build(fileStorage);
        }

        public async Task Authenticate()
        {
            ConnectionParams connectionParams = new ConnectionParams.Builder((await _config).SpotifyClientId)
            .SetRedirectUri("https://partyplaylists.azurewebsites.net/Room/SpotifyAuthorized/")
            .ShowAuthView(true)
            .Build();

            var androidContext = Application.Context;
            SpotifyAppRemote.Connect(androidContext, connectionParams, _connector);
        }

        public void PlaySong(string songId, string roomId)
        {
            try
            {
                _connector.SpotifyApp.PlayerApi.Play(songId);
                var sub = _connector.SpotifyApp.PlayerApi.SubscribeToPlayerState();
                sub.SetEventCallback(new SpotifyCallBack(roomId));
            }
            catch (Exception ex) { }
        }
    }
}