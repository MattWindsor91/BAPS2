using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     A message-send loop for BapsNet.
    /// </summary>
    public class Sender
    {
        [NotNull] private readonly IPrimitiveSink _primitiveSink;

        [ItemNotNull] [NotNull]
        private readonly BlockingCollection<MessageBuilder> _queue = new BlockingCollection<MessageBuilder>();

        private readonly CancellationToken _token;

        /// <summary>
        ///     Constructs a new <see cref="Sender" />.
        /// </summary>
        /// <param name="primitiveSink">
        ///     The <see cref="IPrimitiveSink" /> that the <see cref="Sender" /> will
        ///     send packed BapsNet messages on.
        /// </param>
        /// <param name="token">
        ///     The cancellation token that the <see cref="Sender" /> will check
        ///     to see if it should shut down.
        /// </param>
        public Sender(IPrimitiveSink? primitiveSink, CancellationToken token)
        {
            _token = token;
            _primitiveSink = primitiveSink ?? throw new ArgumentNullException(nameof(primitiveSink));
        }

        /// <summary>
        ///     Queues up a message to send through this <see cref="Sender" />.
        /// </summary>
        /// <param name="messageBuilder">The message to send.</param>
        public void Enqueue(MessageBuilder? messageBuilder)
        {
            if (messageBuilder != null) _queue.Add(messageBuilder, _token);
        }

        /// <summary>
        ///     Runs the sender loop.
        /// </summary>
        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var msg = _queue.Take(_token);
                msg.Send(_primitiveSink);
            }

            _token.ThrowIfCancellationRequested();
        }
    }
}