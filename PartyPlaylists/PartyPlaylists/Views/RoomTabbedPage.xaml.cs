using PartyPlaylists.Models.DataModels;
using PartyPlaylists.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PartyPlaylists.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomTabbedPage : TabbedPage
    {
        public RoomTabbedPage()
        {
            InitializeComponent();
        }

        public RoomTabbedPage(Room room) : this()
        {
            BindingContext = new RoomViewModel(room);
        }
    }
}