using System;
using System.Collections.Concurrent;
using System.Reactive;

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
        private readonly ConcurrentDictionary<ClientHandle, Unit> _clients = new ConcurrentDictionary<ClientHandle, Unit>();

        public void Add(ClientHandle clientHandle)
        {
            if (_clients.TryAdd(clientHandle, Unit.Default)) SetUpNewClient(clientHandle);

        }

        private void SetUpNewClient(ClientHandle clientHandle)
        {
            
        }

        public void Dispose()
        {
            foreach (var (client, _) in _clients)
            {
                client.Dispose();
            }
        }
    }
}