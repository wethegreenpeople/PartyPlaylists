using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Spotify.Android.Appremote.Api;
using Java.Interop;
using Java.Lang;

namespace PartyPlaylists.Droid
{
    public class Connector : Java.Lang.Object, IConnector, IConnectorConnectionListener
    {
        public SpotifyAppRemote SpotifyApp { get; private set; }
        public delegate void EventHandler(object source, MyEventArgs e);
        public static event EventHandler OnConnectedEvent;

        static readonly JniPeerMembers _members = new XAPeerMembers("com/spotify/android/appremote/api/SpotifyAppRemote", typeof(SpotifyAppRemote));
        internal static new IntPtr class_ref
        {
            get
            {
                return _members.JniPeerType.PeerReference.Handle;
            }
        }
        public override JniPeerMembers JniPeerMembers
        {
            get { return _members; }
        }

        protected override IntPtr ThresholdClass
        {
            get { return _members.JniPeerType.PeerReference.Handle; }
        }

        protected override global::System.Type ThresholdType
        {
            get { return _members.ManagedPeerType; }
        }

        public void Connect(Context p0, ConnectionParams p1, IConnectorConnectionListener p2)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(SpotifyAppRemote p0)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Disposed()
        {
            throw new NotImplementedException();
        }

        public void DisposeUnlessReferenced()
        {
            throw new NotImplementedException();
        }

        public void Finalized()
        {
            throw new NotImplementedException();
        }

        public void OnConnected(SpotifyAppRemote p0)
        {
            SpotifyApp = p0;
        }

        public void OnFailure(Throwable p0)
        {
            
        }

        public void SetJniIdentityHashCode(int value)
        {
            throw new NotImplementedException();
        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {
            throw new NotImplementedException();
        }

        public void SetPeerReference(JniObjectReference reference)
        {
            throw new NotImplementedException();
        }

        public void UnregisterFromRuntime()
        {
            throw new NotImplementedException();
        }
    }

    public class MyEventArgs : EventArgs
    {
        private SpotifyAppRemote AppRemote;
        public MyEventArgs(SpotifyAppRemote appRemote)
        {
            AppRemote = appRemote;
        }
        public SpotifyAppRemote GetAppRemote()
        {
            return AppRemote;
        }
    }
}