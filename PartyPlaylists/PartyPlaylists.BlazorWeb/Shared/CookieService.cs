using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPlaylists.BlazorWeb.Shared
{
    public class CookieService
    {
        public static CookieOptions CreateCookieOptions(int? expireTime = null)
        {
            CookieOptions options = new CookieOptions();

            if (expireTime.HasValue)
                options.Expires = DateTime.Now.AddHours(expireTime.Value);
            else
                options.Expires = DateTime.Now.AddHours(1);

            return options;
        }
    }
}
