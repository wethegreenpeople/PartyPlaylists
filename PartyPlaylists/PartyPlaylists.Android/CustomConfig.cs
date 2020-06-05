using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PartyPlaylists.Services;

namespace PartyPlaylists.Droid
{
    public class CustomConfig : ICustomConfig
    {
        public string PartyPlaylistsKey { get; set; }
        public string SpotifyClientId { get; set; }

        public async Task<ICustomConfig> Build(IFileStorage fileStorage)
        {
            try
            {
                var configurationFile = await fileStorage.ReadAsString("appsettings.json");
                var fileObjects = JObject.Parse(configurationFile);

                var configuration = new CustomConfig()
                {
                    PartyPlaylistsKey = fileObjects["Keys"]["PartyPlaylists"]["Key"].ToString(),
                    SpotifyClientId = fileObjects["Keys"]["Spotify"]["ClientId"].ToString(),
                };

                return configuration;
            }
            catch (Exception ex)
            {
                return new CustomConfig();
            }
        }
    }
}