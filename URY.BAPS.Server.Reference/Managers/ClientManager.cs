using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using URY.BAPS.Common.Protocol.V2.MessageIo;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;
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
        private readonly ClientPool _clients = new ClientPool();

        private readonly ConnectionFactory<ClientHandle> _clientFactory;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ChannelSet _channels;

        /// <summary>
        ///     The cancellation token that will be cancelled if the server
        ///     finds itself needing to quit.
        /// </summary>
        public CancellationToken Token => _cts.Token;

        private ILogger<ClientManager> Logger { get; }

        public ClientManager(ILoggerFactory loggerFactory, TcpServer server, ChannelSet channels, ConnectionFactory<ClientHandle> clientFactory)
        {
            _server = server;
            _channels = channels;
            _clientFactory = clientFactory;
            Logger = loggerFactory.CreateLogger<ClientManager>();
        }

        /// <summary>
        ///     Runs the client manager's main loop.
        /// </summary>
        public void Run()
        {
            Logger.LogInformation("ClientManager now running.");

            var observable = _server.ObserveNewConnection;

            var connectionHandleTask = observable.ForEachAsync(HandleNewConnection, Token);
            var serverTask = _server.RunAsync(Token);
            
            connectionHandleTask.Wait(CancellationToken.None);
            serverTask.Wait(CancellationToken.None);
        }

        private void HandleNewConnection(TcpClient e)
        {
            Logger.LogInformation("New client connection: {0}", e);

            ClientHandle? client = null;
            try
            {
                client = _clientFactory.Build(e);
                _clients.Add(client);
                client = null;
            } finally {
                client?.Dispose();
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _clients.Dispose();
        }
    }
}
