using System;
using System.Collections.Concurrent;
using System.Reactive;
using URY.BAPS.Common.Model.EventFeed;

namespace URY.BAPS.Server.Io
{
    /// <summary>
    ///     A thread-safe pool of <see cref="ClientHandle"/>s.
    /// </summary>
    public sealed class ClientPool : IDisposable
    {
        /// <summary>
        ///     The current pool of connected clients.
        /// </summary>
        private readonly ConcurrentDictionary<ClientHandle, IDisposable> _clients = new ConcurrentDictionary<ClientHandle, IDisposable>();

        public IFullEventFeed RequestFeed => _requestFeed;
        
        private readonly DetachableEventFeed _requestFeed = new DetachableEventFeed();
        
        public void Add(ClientHandle? clientHandle)
        {
            if (clientHandle == null) return;
            
            IDisposable? eventSubscription = null;
            try
            {
                eventSubscription = _requestFeed.Attach(clientHandle.RawEventFeed);
                
                // Stops double disposal on the 'finally' path.
                if (_clients.TryAdd(clientHandle, eventSubscription)) eventSubscription = null;
            }
            finally
            {
                eventSubscription?.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var (client, subscription) in _clients)
            {
                client.Dispose();
                _requestFeed.Detach(subscription);
            }
        }
    }
}