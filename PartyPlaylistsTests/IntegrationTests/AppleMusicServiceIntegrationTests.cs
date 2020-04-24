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

        private string _testJwt;
        private IConfiguration _config;

        [TestInitialize]
        public void TestInit()
        {
            _testJwt = AppleMusicService.CreateSignedJwt(_testKey, _testKeyId, _testTeamId);
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
        public async Task GetSongs_Config_OK()
        {
            var amService = new AppleMusicService(_config) as IStreamingService;

            var songs = await amService.GetSongs("test");

            Assert.IsNotNull(songs);
            Assert.IsTrue(songs.Count > 1);
        }

        [TestMethod]
        public async Task GetSongs_Key_TooManyRequests()
        {
            var amService = new AppleMusicService(_testKey, _testKeyId, _testTeamId) as IStreamingService;

            // TODO flush out any preliminary OK responses? Sometimes there is no exception thrown...
            for (int i = 0; i < 3; i++) { try { await amService.GetSongs("test"); } catch { } Thread.Sleep(100); }

            var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await amService.GetSongs("test"));
            Assert.IsTrue(ex.Message.Contains("Response status not OK. Too Many Requests."));
        }

        [TestMethod]
        public async Task GetSongs_Jwt_TooManyRequests()
        {
            var amService = new AppleMusicService(_testJwt) as IStreamingService;

            // TODO flush out any preliminary OK responses? Sometimes there is no exception thrown...
            for (int i = 0; i < 3; i++) { try { await amService.GetSongs("test"); } catch { } Thread.Sleep(100); }

            var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await amService.GetSongs("test"));
            Assert.IsTrue(ex.Message.Contains("Response status not OK. Too Many Requests."));
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
