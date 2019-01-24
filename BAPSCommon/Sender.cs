using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BAPSCommon
{
    /// <summary>
    /// A message-send loop for BAPSnet.
    /// </summary>
    public class Sender
    {
        private CancellationToken _token;

        private BlockingCollection<Message> _queue;

        private ClientSocket _socket;

        /// <summary>
        /// Constructs a new <see cref="Sender"/>.
        /// </summary>
        /// <param name="queue">
        /// The message queue from which the <see cref="Sender"/> will receive
        /// BAPSnet messages.
        /// </param>
        /// <param name="token">
        /// The cancellation token that the <see cref="Sender"/> will check
        /// to see if it should shut down.
        /// </param>
        /// <param name="socket">
        /// The <see cref="ClientSocket"/> that the <see cref="Sender"/> will
        /// send packed BAPSnet messages on.
        /// </param>
        public Sender(BlockingCollection<Message> queue, CancellationToken token, ClientSocket socket)
        {
            _token = token;
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        /// <summary>
        /// Runs the sender loop.
        /// </summary>
        public void Run()
        {
            try
            {
                while (true)
                {
                    _token.ThrowIfCancellationRequested();
                    var msg = _queue.Take(_token);
                    msg.Send(_socket);
                }
            }
            finally
            {
                _socket.ShutdownSend();
            }
        }
    }
}
