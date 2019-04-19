using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     A message-send loop for BapsNet.
    /// </summary>
    public class Sender
    {
        [ItemNotNull] [NotNull] private readonly BlockingCollection<Message> _queue = new BlockingCollection<Message>();

        [NotNull] private readonly ISink _sink;
        private readonly CancellationToken _token;

        /// <summary>
        ///     Constructs a new <see cref="Sender" />.
        /// </summary>
        /// <param name="sink">
        ///     The <see cref="ISink" /> that the <see cref="Sender" /> will
        ///     send packed BapsNet messages on.
        /// </param>
        /// <param name="token">
        ///     The cancellation token that the <see cref="Sender" /> will check
        ///     to see if it should shut down.
        /// </param>
        public Sender(ISink? sink, CancellationToken token)
        {
            _token = token;
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        }

        /// <summary>
        ///     Queues up a message to send through this <see cref="Sender" />.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Enqueue(Message? message)
        {
            if (message != null) _queue.Add(message, _token);
        }

        /// <summary>
        ///     Runs the sender loop.
        /// </summary>
        public void Run()
        {
            while (!_token.IsCancellationRequested)
            {
                var msg = _queue.Take(_token);
                msg.Send(_sink);
            }
            _token.ThrowIfCancellationRequested();
        }
    }
}