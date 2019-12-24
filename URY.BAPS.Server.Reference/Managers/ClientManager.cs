using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Server.Io;
using URY.BAPS.Server.Model;

namespace URY.BAPS.Server.Managers
{
    public sealed class ClientManager : IDisposable
    {
        private readonly TcpServer _server;

        /// <summary>
        ///     The current pool of connected clients.
        /// </summary>
        private readonly ConcurrentDictionary<Client, bool> _clients = new ConcurrentDictionary<Client, bool>();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ChannelSet _channels;

        /// <summary>
        ///     The cancellation token that will be cancelled if the server
        ///     finds itself needing to quit.
        /// </summary>
        public CancellationToken Token => _cts.Token;

        private ILogger<ClientManager> Logger { get; }

        public ClientManager(ILoggerFactory loggerFactory, TcpServer server, ChannelSet channels)
        {
            _server = server;
            _channels = channels;
            Logger = loggerFactory.CreateLogger<ClientManager>();
        }

        /// <summary>
        ///     Runs the client manager's main loop.
        /// </summary>
        public void Run()
        {
            Logger.LogInformation("ClientManager now running.");

            var observable = _server.ObserveNewConnection;

            observable.ForEachAsync(HandleNewConnection, Token).Wait(CancellationToken.None);

        }

        private void HandleNewConnection(TcpConnection e)
        {
            var client = new Client(e);
            if (!_clients.TryAdd(client, true))
            {
                Logger.LogError("Couldn't add client {0} to client pool");
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            foreach (var (client, _) in _clients)
            {
                client.Dispose();
            }
        }
    }
}
