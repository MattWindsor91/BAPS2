using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;
using URY.BAPS.Server.Config;

namespace URY.BAPS.Server.Io
{
    /// <summary>
    ///     A TCP server that listens on the configured host and port, and
    ///     produces <see cref="TcpClient"/>s.
    /// </summary>
    public class TcpServer
    {
        private readonly TcpListener _listener;
        private IObservable<TcpClient>? _observeNewConnection;
        private ILogger<TcpServer> Logger { get; }

        /// <summary>
        ///     Event triggered when a new connection is accepted.
        /// </summary>
        public event EventHandler<TcpClient>? NewConnection;

        public IObservable<TcpClient> ObserveNewConnection =>
            _observeNewConnection ??= Observable.FromEventPattern<TcpClient>(e => NewConnection += e, e => NewConnection -= e)
                .Select(x => x.EventArgs);

        public TcpServer(ILoggerFactory loggerFactory, ListenConfig config)
        {
            _listener = new TcpListener(config.HostEndPoint);

            Logger = loggerFactory.CreateLogger<TcpServer>();
        }

        public async Task RunAsync(CancellationToken token)
        {
            _listener.Start();
            Logger.LogInformation("Started listening.");

            try
            {
                await MainLoop(token).ConfigureAwait(false);
            }
            finally
            {
                _listener.Stop();
                Logger.LogInformation("Stopped listening.");
            }            
        }

        private async Task MainLoop(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(true);

                OnNewConnection(client);
            }
        }

        protected virtual void OnNewConnection(TcpClient e)
        {
            NewConnection?.Invoke(this, e);
        }
    }
}
