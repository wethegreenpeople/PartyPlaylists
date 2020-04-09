using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PartyPlaylists.MVC.Models;
using PartyPlaylists.M
using PartyPlaylists.MVC.DataAccess;
using MySql.Data.MySqlClient;

namespace PartyPlaylistsTests.IntegrationTests
{
    [TestClass]
    public class PartyPlaylistsDataAccessTests
    {

        [TestInitialize]
        public void TestInit ()
        {
            
        }

        [TestMethod]
        public void PlaylistsMvcDataBaseConnectionTest ()
        {
            DbContextOptionsBuilder<PlaylistContext> dbContextOptions = new DbContextOptionsBuilder<PlaylistContext>().UseMySql(PlaylistContext.CreateConnectionString(true));
            using (PlaylistContext playlistContext = new PlaylistContext(dbContextOptions.Options))
            {
                Assert.IsTrue(playlistContext.Database.CanConnect());
            }
        }
    }
}
