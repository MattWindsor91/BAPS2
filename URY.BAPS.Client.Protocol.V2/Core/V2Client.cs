using URY.BAPS.Client.Common.Login;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.ServerSelect;
using URY.BAPS.Client.Protocol.V2.Login;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.MessageIo;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Performs all of the core functionality of starting, managing, and
    ///     stopping a BapsNet v2 client.
    ///     <para>
    ///         Objects of this class compose several client-related objects
    ///         into one place.
    ///     </para>
    /// </summary>
    public class V2Client
    {
        private readonly MessageConnectionManager _connectionManager;
        private readonly ConfigCache _configCache;
        private readonly InitialUpdatePerformer _init;
        private readonly ServerSelector _serverSelector;
        private readonly ClientSideLoginPerformer<SeededPrimitiveConnection, IMessageConnection> _login;

        /// <summary>
        ///     An event feed that receives updates from the BAPS server.
        /// </summary>
        public IFullEventFeed EventFeed => _connectionManager.EventFeed;

        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void Send(MessageBuilder? messageBuilder)
        {
            _connectionManager.Send(messageBuilder);
        }

        /// <summary>
        ///     Constructs a <see cref="V2Client"/>.
        ///     <para>
        ///         This constructor takes in a lot of dependencies; generally,
        ///         code using <see cref="V2Client"/> will just ask for one from
        ///         a dependency injector rather than manually constructing
        ///         one.
        ///     </para>
        /// </summary>
        /// <param name="connectionManager">
        ///     A <see cref="MessageConnectionManager"/>, used to build and hold onto
        ///     message-passing connections to a BAPS server.
        /// </param>
        /// <param name="configCache">
        ///     A server configuration cache, used to hold onto config
        ///     information received from the BAPS server.
        /// </param>
        /// <param name="init">
        ///     An initial update performer, used to send certain set-up
        ///     messages to a BAPS server on connecting to one.
        /// </param>
        /// <param name="serverSelector">
        ///     An object that performs server selection.
        /// </param>
        /// <param name="login">
        ///     An object that performs authentication for low-level connections to the BAPS server, returning
        ///     <see cref="TcpPrimitiveConnection"/>s that are then sent to the
        ///     <paramref name="connectionManager"/>.
        /// </param>
        public V2Client(MessageConnectionManager connectionManager, ConfigCache configCache, InitialUpdatePerformer init,
            ServerSelector serverSelector,
            ClientSideLoginPerformer<SeededPrimitiveConnection, IMessageConnection> login)
        {
            _connectionManager = connectionManager;
            _configCache = configCache;
            _init = init;
            _serverSelector = serverSelector;
            _login = login;
        }

        /// <summary>
        ///     Tries to get an authenticated BapsNet connection, then, if
        ///     authentication succeeded, spins up the send and receive tasks.
        /// </summary>
        /// <returns>
        ///     True if the client was successfully launched; false otherwise.
        /// </returns>
        public bool Start()
        {
            _configCache.SubscribeToReceiver(EventFeed);

            _serverSelector.Run();
            if (!_serverSelector.HasConnection) return false;

            _login.TryLogin(_serverSelector.Connection);
            if (!_login.HasConnection) return false;
            
            _connectionManager.Launch(_login.Connection);
            _init.Run();
            return true;
        }

        /// <summary>
        ///     Shuts down the client.
        /// </summary>
        public void Stop()
        {
            NotifyServerOfQuit();
            _connectionManager.Shutdown();
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = new SystemCommand(SystemOp.End);
            _connectionManager.TrySend(new MessageBuilder(cmd).Add("Normal Termination"));
        }
    }
}