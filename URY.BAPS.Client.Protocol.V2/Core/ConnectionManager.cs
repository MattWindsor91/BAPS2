using System;
using System.Threading;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Manages a single message-based BapsNet connection.
    ///     <para>
    ///         This object internally tracks a single connection, which can
    ///         be set up using <see cref="Launch"/> and shut down using
    ///         <see cref="Shutdown"/>.
    ///     </para>
    /// </summary>
    public sealed class ConnectionManager
    {
        private Connection? _connection;

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
            _connection.Send(messageBuilder);
        }

        /// <summary>
        ///     Attaches this <see cref="ConnectionManager"/> to a connection to a
        ///     BapsNet server, expressed as a pair of primitive source and
        ///     primitive sink, and starts the send and receive loops.
        /// </summary>
        /// <param name="source">
        ///     The primitive source from which BapsNet commands will be read.
        /// </param>
        /// <param name="sink">
        ///     The primitive sink to which BapsNet commands will be written.
        /// </param>
        public void Launch(IPrimitiveSource source, IPrimitiveSink sink)
        {
            if (_connection != null)
                throw new InvalidOperationException("Already launched.");
            _connection = new Connection(source, sink, MakeClientCommandDecoder);
            _connection.AttachToReceiver(_eventFeed);
            _connection.StartLoops();
        }

        private static CommandDecoder MakeClientCommandDecoder(IPrimitiveSource source, CancellationToken token)
        {
            return new ClientCommandDecoder(source, token);
        }

        public void Shutdown()
        {
            if (_connection == null)
                throw new InvalidOperationException("Already shut down.");

            _connection.StopLoops();
            _eventFeed.Detach();
            _connection = null;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}