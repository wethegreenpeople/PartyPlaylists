using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PartyPlaylists.Services;

namespace PartyPlaylists.Droid
{
    public class FileStorage : IFileStorage
    {
        private Context _context = Application.Context;
        public async Task<string> ReadAsString(string filename)
        {
            using (var asset = _context.Assets.Open(filename))
            using (var streamReader = new StreamReader(asset))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}