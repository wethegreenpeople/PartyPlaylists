using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PartyPlaylists.Models;
using PartyPlaylists.Models.DataModels;
using System.IO;

namespace PartyPlaylists.MobileAppService.Contexts
{
    public class PlaylistContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SpotifyPlaylist> SpotifyPlaylist { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<RoomSong> RoomSongs { get; set; }
        public DbSet<RoomSongToken> RoomSongTokens { get; set; }

        public PlaylistContext(DbContextOptions<PlaylistContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomSong>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<RoomSong>()
                .HasOne(e => e.Room)
                .WithMany(e => e.RoomSongs)
                .HasForeignKey(e => e.RoomId);
            modelBuilder.Entity<RoomSong>()
                .HasOne(e => e.Song)
                .WithMany(e => e.RoomSongs)
                .HasForeignKey(e => e.SongId);

            modelBuilder.Entity<RoomSongToken>()
               .HasKey(e => e.Id);
            modelBuilder.Entity<RoomSongToken>()
                .HasOne(e => e.Token)
                .WithMany(e => e.RoomSongTokens)
                .HasForeignKey(e => e.TokenId);
            modelBuilder.Entity<RoomSongToken>()
                .HasOne(e => e.RoomSong)
                .WithMany(e => e.RoomSongTokens)
                .HasForeignKey(e => e.RoomSongId);

            modelBuilder.Entity<Room>()
                .HasOne(sp => sp.SpotifyPlaylist)
                .WithOne(r => r.Room)
                .HasForeignKey<SpotifyPlaylist>(r => r.RoomId);

            modelBuilder.Entity<Token>()
                .HasIndex(s => s.JWTToken)
                .IsUnique();


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
            optionsBuilder.UseMySql(connectionString);
            return new PlaylistContext(optionsBuilder.Options);
        }
    }
}
