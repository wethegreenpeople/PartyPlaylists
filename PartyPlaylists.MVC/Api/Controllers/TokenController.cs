using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.Services;

namespace PartyPlaylists.MVC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly PlaylistContext _playlistContext;
        private readonly IConfiguration _config;

        public TokenController(PlaylistContext context, IConfiguration config)
        {
            _config = config;
            _playlistContext = context;
        }

        [AllowAnonymous]
        [HttpPost("{apiKey}")]
        public async Task<ActionResult<string>> CreateToken(string apiKey)
        {
            if (!_playlistContext.ApiKeys.Any(s => s.Key == apiKey))
                return Unauthorized();

            var token = await TokenService.CreateTokenAsync(_config);
            await new TokenService(_playlistContext, _config).SaveTokenAsync(token);

            return token;
        }
    }
}