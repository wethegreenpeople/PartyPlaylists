using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.EntityFrameworkCore;
using PartyPlaylists.Contexts;
using Splat;
using Microsoft.AspNetCore.SignalR.Client;
using PartyPlaylists.Services;

namespace PartyPlaylists.Droid
{
    [Activity(Label = "PartyPlaylists", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new HubConnectionBuilder().WithUrl($"https://partyplaylists.azurewebsites.net/roomhub").Build(), typeof(HubConnection));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SpotifyMobileSDK(), typeof(ISpotifyMobileSDK));
            Locator.CurrentMutable.RegisterLazySingleton(() => new CustomConfig(), typeof(ICustomConfig));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FileStorage(), typeof(IFileStorage));
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}