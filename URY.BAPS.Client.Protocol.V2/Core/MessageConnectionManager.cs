using System;
using System.Threading;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Common.Protocol.V2.MessageIo
{
    /// <summary>
    ///     Wraps a single message-based BapsNet connection, providing the
    ///     ability to subscribe to its messages before the connection is
    ///     made.
    ///     <para>
    ///         This object internally tracks a single connection, which can
    ///         be set up using <see cref="Launch"/> and shut down using
    ///         <see cref="Shutdown"/>.
    ///     </para>
    /// </summary>
    public sealed class MessageConnectionManager : IDisposable
    {
        /// <summary>
        ///     The underlying connection, if one has been created.
        /// </summary>
        private IMessageConnection? _connection;

        private readonly DetachableEventFeed _eventFeed = new DetachableEventFeed();

        /// <summary>
        ///     An event feed that receives updates from the BAPS server.
        /// </summary>
        public IFullEventFeed EventFeed => _eventFeed;

        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void Send(MessageBuilder? messageBuilder)
        {
            if (_connection == null)
                throw new InvalidOperationException("Tried to send without a connection.");
            TrySend(messageBuilder);
        }

        /// <summary>
        ///     Sends a message to the BapsNet server, silently discarding if
        ///     there is no active connection.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void TrySend(MessageBuilder? messageBuilder)
        {
            _connection?.Send(messageBuilder);
        }

        /// <summary>
        ///     Attaches this <see cref="MessageConnectionManager"/> to another message-level connection,
        ///     starting its loops.
        /// </summary>
        /// <param name="messageConnection">
        ///     The connection to attach to the detachable connection.
        /// </param>
        public void Launch(IMessageConnection messageConnection)
        {
            if (_connection != null)
                throw new InvalidOperationException("This detachable connection already has a connection attached.");
            _connection = messageConnection ?? throw new ArgumentNullException(nameof(messageConnection));
            
            _eventFeed.Attach(_connection.RawEventFeed);
            _connection.StartLoops();
        }
        
        /// <summary>
        ///     Shuts down any active connection.
        ///     <para>
        ///         It should be safe to call this multiple times, or call it
        ///         before a connection is established.
        ///     </para>
        /// </summary>
        public void Shutdown()
        {
            _eventFeed.DetachAll();
            _connection = null;
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _eventFeed.Dispose();
        }
    }
}