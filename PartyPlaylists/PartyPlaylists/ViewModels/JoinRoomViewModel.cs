using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;
using PartyPlaylists.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Splat;
using Microsoft.EntityFrameworkCore;
using SpotifyApi.NetCore;

namespace PartyPlaylists.ViewModels
{
    public class JoinRoomViewModel : BaseViewModel
    {
        private readonly RoomDataStore _roomDataStore;

        public Command JoinRoomCommand { get; set; }

        public JoinRoomViewModel()
        {
            Title = "Join a Room";

            JoinRoomCommand = new Command(() => JoinRoom());
        }

        private void JoinRoom()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var context = Locator.Current.GetService<PlaylistContext>();
            try
            {
                var options = new DbContextOptionsBuilder<PlaylistContext>().UseMySql("Server = 40.117.143.83; Database = PartyPlaylists; Uid = remoteuser; Pwd = password;").Options;
                using (var doot = new PlaylistContext(options))
                {
                    var roomds = new RoomDataStore(doot);
                    var room = roomds.GetItemAsync("295").Result;

                    RootPage.Detail.Navigation.PushAsync(new RoomPage(room));
                }
            }
            catch (Exception ex)
            {

            }
            
            IsBusy = false;
        }
    }
}