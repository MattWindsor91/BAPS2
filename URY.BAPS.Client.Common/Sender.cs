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

        [NotNull] private readonly ClientSocket _socket;
        private readonly CancellationToken _token;

        /// <summary>
        ///     Constructs a new <see cref="Sender" />.
        /// </summary>
        /// <param name="token">
        ///     The cancellation token that the <see cref="Sender" /> will check
        ///     to see if it should shut down.
        /// </param>
        /// <param name="socket">
        ///     The <see cref="ClientSocket" /> that the <see cref="Sender" /> will
        ///     send packed BAPSnet messages on.
        /// </param>
        public Sender(CancellationToken token, ClientSocket socket)
        {
            _token = token;
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        ///     Asynchronously sends a message through this <see cref="Sender" />.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendAsync([CanBeNull] Message message)
        {
            if (message != null) _queue.Add(message, _token);
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