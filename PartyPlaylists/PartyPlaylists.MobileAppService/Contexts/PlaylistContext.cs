using Microsoft.EntityFrameworkCore;
using PartyPlaylists.MobileAppService.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MobileAppService.Contexts
{
    public class PlaylistContext : DbContext
    {
        public PlaylistContext(DbContextOptions<PlaylistContext> options) : base(options)
        {

        }

        public DbSet<Room> Rooms { get; set; }
    }
}
