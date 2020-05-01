using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylists.Models
{
    public enum MenuItemType
    {
        Join,
        Create,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
