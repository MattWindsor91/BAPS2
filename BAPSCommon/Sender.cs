using System;
using System.Collections.Concurrent;
using System.Threading;
using BAPSClientCommon.BapsNet;
using JetBrains.Annotations;

namespace BAPSClientCommon
{
    /// <summary>
    ///     A message-send loop for BAPSnet.
    /// </summary>
    public class Sender
    {
        [NotNull] private readonly BlockingCollection<Message> _queue;

        [NotNull] private readonly ClientSocket _socket;
        private readonly CancellationToken _token;

        /// <summary>
        ///     Constructs a new <see cref="Sender" />.
        /// </summary>
        /// <param name="queue">
        ///     The message queue from which the <see cref="Sender" /> will receive
        ///     BAPSnet messages.
        /// </param>
        /// <param name="token">
        ///     The cancellation token that the <see cref="Sender" /> will check
        ///     to see if it should shut down.
        /// </param>
        /// <param name="socket">
        ///     The <see cref="ClientSocket" /> that the <see cref="Sender" /> will
        ///     send packed BAPSnet messages on.
        /// </param>
        public Sender(BlockingCollection<Message> queue, CancellationToken token, ClientSocket socket)
        {
            _token = token;
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        ///     Runs the sender loop.
        /// </summary>
        public void Run()
        {
            try
            {
                while (!_token.IsCancellationRequested)
                {
                    var msg = _queue.Take(_token);
                    msg.Send(_socket);
                }

                _token.ThrowIfCancellationRequested();
            }
            finally
            {
                _socket.ShutdownSend();
            }
        }
    }
}