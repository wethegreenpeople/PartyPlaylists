using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PartyPlaylists.MobileAppService.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.MobileAppService.Contexts
{
    public class PlaylistContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }

        public PlaylistContext(DbContextOptions<PlaylistContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class PlaylistContextFactory : IDesignTimeDbContextFactory<PlaylistContext>
    {
        public PlaylistContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PlaylistContext>();
            var connectionString = configuration.GetConnectionString("PartyPlaylistConnectionString");
            optionsBuilder.UseMySQL(connectionString);
            return new PlaylistContext(optionsBuilder.Options);
        }
    }
}
