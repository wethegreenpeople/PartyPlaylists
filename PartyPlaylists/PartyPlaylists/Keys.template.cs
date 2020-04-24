using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: This is a VERY unfortunate way of keeping some secrets out
// of the repo. It has duplicated information from our standard appsettings.json file
// which already gets duplicated far too many times.
// We need a better way of implementing both this file, and the appsettings.json in general.

// Remove .template from the namespace, and file name
// Fill in whatever key portions you need, and you should be good to go.
namespace PartyPlaylists.Droid.Template
{
    public static class Keys
    {
        public static string JwtKey => "";
    }
}
