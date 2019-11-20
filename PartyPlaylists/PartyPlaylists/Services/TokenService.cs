using PartyPlaylists.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlaylists.Services
{
    public class TokenService
    {
        private readonly HttpClient _client;

        public TokenService()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(@"http://40.117.143.83/partyplaylists/api/")
            };
        }

        public async Task<string> GetToken()
        {
            return await _client.GetStringAsync($@"token");
        }
    }
}
