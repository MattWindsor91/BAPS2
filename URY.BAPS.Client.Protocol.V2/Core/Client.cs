using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;
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
    public class Client
    {
        private readonly ConnectionManager _connection;
        private readonly ConfigCache _configCache;
        private readonly InitialUpdatePerformer _init;
        private readonly Authenticator<TcpConnection> _auth;

        /// <summary>
        ///     An event feed that receives updates from the BAPS server.
        /// </summary>
        public IFullEventFeed EventFeed => _connection.EventFeed;

        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void Send(MessageBuilder? messageBuilder)
        {
            _connection.Send(messageBuilder);
        }

        /// <summary>
        ///     Constructs a <see cref="Client"/>.
        ///     <para>
        ///         This constructor takes in a lot of dependencies; generally,
        ///         code using <see cref="Client"/> will just ask for one from
        ///         a dependency injector rather than manually constructing
        ///         one.
        ///     </para>
        /// </summary>
        /// <param name="connection">
        ///     A <see cref="ConnectionManager"/>, used to build and hold onto
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
        /// <param name="auth">
        ///     An object that performs authentication for low-level
        ///     connections to the BAPS server, returning
        ///     <see cref="TcpConnection"/>s that are then sent to the
        ///     <paramref name="connection"/>.
        /// </param>
        public Client(ConnectionManager connection, ConfigCache configCache, InitialUpdatePerformer init,
            Authenticator<TcpConnection> auth)
        {
            _connection = connection;
            _configCache = configCache;
            _init = init;
            _auth = auth;
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

            var socket = _auth.Run();
            if (socket is null) return false;

            _connection.Launch(socket, socket);
            _init.Run();
            return true;
        }

        /// <summary>
        ///     Shuts down the client.
        /// </summary>
        public void Stop()
        {
            NotifyServerOfQuit();
            _connection.Shutdown();
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = new SystemCommand(SystemOp.End);
            _connection.TrySend(new MessageBuilder(cmd).Add("Normal Termination"));
        }
    }
}