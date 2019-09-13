using System;
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

        public Client(ConnectionManager connection, ConfigCache configCache, InitialUpdatePerformer init, Authenticator<TcpConnection> auth)
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
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = new SystemCommand(SystemOp.End);
            Send(new MessageBuilder(cmd).Add("Normal Termination"));
        }
    }
}