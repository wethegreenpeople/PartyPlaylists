using System;
using Java.Interop;
using Microsoft.AspNetCore.SignalR.Client;
using Splat;
using static Com.Spotify.Protocol.Client.Subscription;

namespace PartyPlaylists.Droid
{
    public class SpotifyCallBack : Java.Lang.Object, IEventCallback
    {
        private readonly HubConnection _hubConnection = Locator.Current.GetService<HubConnection>();
        
        public string CurrentRoomId { get; set; }

        public SpotifyCallBack()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
                _hubConnection.StartAsync();
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

        public async void OnEvent(Java.Lang.Object p0)
        {
            if(((Com.Spotify.Protocol.Types.PlayerState)p0).IsPaused && !string.IsNullOrEmpty(CurrentRoomId))
            {
                await _hubConnection.InvokeAsync("PlayNextSongAsync", CurrentRoomId);
            }
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
}