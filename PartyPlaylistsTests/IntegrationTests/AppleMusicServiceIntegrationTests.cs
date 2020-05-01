using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyPlaylists.Services;
using RestSharp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PartyPlaylistsTests.IntegrationTests
{
    [TestClass]
    public class AppleMusicServiceIntegrationTests
    {
        // This is a test key provided by Apple and used for invoking a TooManyRequests response.
        // More info: https://developer.apple.com/documentation/applemusicapi/getting_keys_and_creating_tokens
        private static readonly string _testKey = @"MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgU208KCg/doqiSzsVF5sknVtYSgt8/3oiYGbvryIRrzSgCgYIKoZIzj0DAQehRANCAAQfrvDWizEnWAzB2Hx2r/NyvIBO6KGBDL7wkZoKnz4Sm4+1P1dhD9fVEhbsdoq9RKEf8dvzTOZMaC/iLqZFKSN6";
        private static readonly string _testKeyId = "CapExedKid";
        private static readonly string _testTeamId = "CapExdTeam";
        // This test JSON Web Token expires 9/25/2020 9:12:03 PM UTC
        private static readonly string _testJwt = @"eyJhbGciOiJFUzI1NiIsImtpZCI6IkNhcEV4ZWRLaWQiLCJ0eXAiOiJKV1QifQ.eyJuYmYiOjE1ODc4NDkxMjMsImV4cCI6MTYwMTA2ODMyMywiaWF0IjoxNTg3ODQ5MTIzLCJpc3MiOiJDYXBFeGRUZWFtIn0.AYAD-abqynDb6JD0c44bhVHdO1qF8Qj91UTONz3iMe7aRkf9YtLAJB9ABZIoCWfuhzUGEau26zMFms_b3C62Lw";

        private IConfiguration _config;

        [TestInitialize]
        public void TestInit()
        {
            // Used to generate new test JWT
            //_testJwt = AppleMusicService.CreateSignedJwt(_testKey, _testKeyId, _testTeamId);

            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        [TestMethod]
        public async Task GetResponse_Test_Unauthorized()
        {
            var response = await PerformTestRequest();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetResponse_Test_TooManyRequests()
        {
            var response = await PerformTestRequest(_testJwt);

            Assert.IsNotNull(response);

            if (response.StatusCode == HttpStatusCode.OK)
                Assert.Inconclusive("Rerun the test. Sometimes the initial response is OK.");

            Assert.AreEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
        }

        [TestMethod]
        public async Task GetResponse_Test_OK()
        {
            var key = _config["Keys:AppleMusic:Key"];
            var keyId = _config["Keys:AppleMusic:KeyId"];
            var teamId = _config["Keys:AppleMusic:TeamId"];

            var jwt = AppleMusicService.CreateSignedJwt(key, keyId, teamId);
            var response = await PerformTestRequest(jwt);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetSong_Config_Term_OK()
        {
            var amService = new AppleMusicService(_config) as IStreamingService;

            var song = await amService.GetSong("test");

            Assert.IsNotNull(song);
            Assert.IsTrue(song.ServiceId.Contains("1031354882"));
        }

        [TestMethod]
        public async Task GetSong_Config_Id_OK()
        {
            var amService = new AppleMusicService(_config) as IStreamingService;

            var song = await amService.GetSong("1487502476");

            Assert.IsNotNull(song);
            Assert.IsTrue(song.ServiceId.Contains("1487502476"));
        }

        [TestMethod]
        public async Task GetSongs_Config_OK()
        {
            var amService = new AppleMusicService(_config) as IStreamingService;

            var songs = await amService.GetSongs("test");

            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count > 1);
        }

        private async Task<IRestResponse> PerformTestRequest(string jwt = null)
        {
            var baseUrl = @"https://api.music.apple.com/v1";

            var client = AppleMusicService.CreateRestClient(baseUrl, jwt);

            var request = new RestRequest(@"test", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = await client.ExecuteAsync(request);

            return response;
        }
    }
}
