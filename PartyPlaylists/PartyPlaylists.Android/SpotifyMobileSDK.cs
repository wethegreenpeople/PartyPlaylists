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

namespace PartyPlaylists.Droid
{
    public class SpotifyMobileSDK : ISpotifyMobileSDK
    {
        private readonly Connector _connector;

        public bool IsAuthenticated => _connector.SpotifyApp != null;
        public SpotifyMobileSDK()
        {
            _connector = new Connector();
        }

        public void Authenticate()
        {
            ConnectionParams connectionParams = new ConnectionParams.Builder("<SPOTIFY_CLIENT_ID>")
            .SetRedirectUri("https://partyplaylists.azurewebsites.net/Room/SpotifyAuthorized/")
            .ShowAuthView(true)
            .Build();

            var androidContext = Application.Context;
            SpotifyAppRemote.Connect(androidContext, connectionParams, _connector);
        }

        public void PlaySong(string songId)
        {
            try
            {
                _connector.SpotifyApp.PlayerApi.Play(songId);
            }
            catch (Exception ex) { }
        }
    }
}