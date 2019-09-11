using System;
using System.Threading;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     An object representing a live connection to a BapsNet client, with
    ///     running send and receive loops.
    /// </summary>
    public sealed class Connection
    {
        /// <summary>
        ///     The amount of delay added to the cancellation request when
        ///     the <see cref="StopLoops"/> method is called.
        /// </summary>
        private const int CancelGracePeriodMilliseconds = 500;

        private readonly CancellationTokenSource _dead = new CancellationTokenSource();

        private readonly Receiver _receiver;
        private readonly Sender _sender;
        private ClientTaskHandle? _tasks;

        /// <summary>
        ///     Constructs a <see cref="Connection"/> on top of the given
        ///     BapsNet primitive handlers.
        /// </summary>
        /// <param name="source">The primitive source used to receive commands.</param>
        /// <param name="sink">The primitive sink used to send commands.</param>
        /// <param name="commandDecoderFactory">
        ///     A function that produces command decoders appropriate for the
        ///     role of this connection (client or server).
        /// </param>
        public Connection(IPrimitiveSource source, IPrimitiveSink sink, Func<IPrimitiveSource, CancellationToken, CommandDecoder> commandDecoderFactory)
        {
            _receiver = CreateReceiver(source, commandDecoderFactory);
            _sender = new Sender(sink);
        }

        /// <summary>
        ///     Attaches the given server updater to this connection's
        ///     receiver, causing it to receive decoded server messages.
        /// </summary>
        /// <param name="updater">
        ///     The updater to attach to the receiver.
        /// </param>
        public void AttachToReceiver(DetachableEventFeed updater)
        {
            updater.Attach(_receiver.ObserveMessage);
        }

        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void Send(MessageBuilder? messageBuilder)
        {
            if (messageBuilder != null) _sender.Enqueue(messageBuilder);
        }

        public void StartLoops()
        {
            if (_tasks != null)
                throw new InvalidOperationException("This connection is already running.");
            _tasks = ClientTaskHandle.CreateAndLaunchTasks(_receiver, _sender, _dead.Token);
        }

        private Receiver CreateReceiver(IPrimitiveSource source, Func<IPrimitiveSource, CancellationToken, CommandDecoder> commandDecoderFactory)
        {
            var decoder = commandDecoderFactory(source, _dead.Token);
            return new Receiver(source, decoder, _dead.Token);
        }

        public void StopLoops()
        {
            if (_tasks == null)
                throw new InvalidOperationException("This connection has already shut down.");

            CancelTasks();
            _tasks = null;
        }

        /// <summary>
        ///     Cancels and joins the receiver and sender tasks.
        /// </summary>
        private void CancelTasks()
        {
            _dead.CancelAfter(CancelGracePeriodMilliseconds);
            _tasks?.Wait();
            _tasks?.Dispose();
        }

        public void Dispose()
        {
            _dead?.Dispose();
            _tasks?.Dispose();
        }
    }
}
