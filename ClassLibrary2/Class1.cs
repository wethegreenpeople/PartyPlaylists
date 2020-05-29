using Com.Spotify.Android.Appremote.Api;
using System;

namespace ClassLibrary2
{
    public class Class1
    {
        public Class1()
        {
            ConnectionParams connectionParams = new ConnectionParams.Builder("000")
            .SetRedirectUri("000")
            .ShowAuthView(true)
            .Build();
        }
    }
}
