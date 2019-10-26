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
    public sealed class Sender : IDisposable
    {
        [NotNull] private readonly IPrimitiveSink _primitiveSink;

        [ItemNotNull] [NotNull]
        private readonly BlockingCollection<MessageBuilder> _queue = new BlockingCollection<MessageBuilder>();

        /// <summary>
        ///     Constructs a new <see cref="Sender" />.
        /// </summary>
        /// <param name="primitiveSink">
        ///     The <see cref="IPrimitiveSink" /> that the <see cref="Sender" /> will
        ///     send packed BapsNet messages on.
        /// </param>
        public Sender(IPrimitiveSink? primitiveSink)
        {
            _primitiveSink = primitiveSink ?? throw new ArgumentNullException(nameof(primitiveSink));
        }

        /// <summary>
        ///     Queues up a message to send through this <see cref="Sender" />.
        /// </summary>
        /// <param name="messageBuilder">The message to send.</param>
        public void Enqueue(MessageBuilder? messageBuilder)
        {
            if (messageBuilder != null) _queue.Add(messageBuilder);
        }

        /// <summary>
        ///     Runs the sender loop.
        /// </summary>
        /// <param name="token">
        ///     The cancellation token used to stop the sender from sending.
        /// </param>
        public void Run(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var msg = _queue.Take(token);
                msg.Send(_primitiveSink);
            }
        }

        public void Dispose()
        {
            _queue.Dispose();
        }
    }
}