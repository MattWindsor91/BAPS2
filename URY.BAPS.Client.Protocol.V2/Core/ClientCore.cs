using System;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Manages a message-based BapsNet connection.
    ///     <para>
    ///         This object internally tracks a single connection, which can
    ///         be set up using <see cref="Launch"/> and shut down using
    ///         <see cref="Shutdown"/>.
    ///     </para>
    /// </summary>
    public sealed class ClientCore
    {
        private Connection? _connection;

        private readonly DetachableServerUpdater _updater = new DetachableServerUpdater();
        public IServerUpdater Updater => _updater;

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
        ///     Attaches this <see cref="ClientCore"/> to a connection to a
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
            _connection = new Connection(source, sink);
            _connection.AttachToReceiver(_updater);
            _connection.StartLoops();
        }

        public void Shutdown()
        {
            if (_connection == null)
                throw new InvalidOperationException("Already shut down.");
            NotifyServerOfQuit();
            _connection.StopLoops();
            _updater.Detach();
            _connection = null;
        }


        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = new SystemCommand(SystemOp.End);
            Send(new MessageBuilder(cmd).Add("Normal Termination"));
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}