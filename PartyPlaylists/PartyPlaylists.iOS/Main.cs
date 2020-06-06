using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Microsoft.AspNetCore.SignalR.Client;
using Splat;
using UIKit;

using PartyPlaylists.Services;
namespace PartyPlaylists.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new HubConnectionBuilder().WithUrl($"https://partyplaylists.azurewebsites.net/roomhub").Build(), typeof(HubConnection));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SpotifyMobileSDK(), typeof(ISpotifyMobileSDK));
            Locator.CurrentMutable.RegisterLazySingleton(() => new CustomConfig(), typeof(ICustomConfig));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FileStorage(), typeof(IFileStorage));

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
