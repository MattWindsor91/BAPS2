using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     Object encapsulating the core features of a BapsNet client.
    /// </summary>
    public partial class ClientCore : IClientCore
    {
        private const int CancelGracePeriodMilliseconds = 500;

        private readonly CancellationTokenSource _dead = new CancellationTokenSource();

        private Receiver? _receiver;
        private Task? _receiverTask;

        private Sender? _sender;
        private Task? _senderTask;

        private TcpConnection? _socket;

        /// <inheritdoc />
        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        public void Send(MessageBuilder? messageBuilder)
        {
            if (messageBuilder != null) _sender?.Enqueue(messageBuilder);
        }

        public void Launch(TcpConnection bapsConnection)
        {
            if (_socket != null)
                throw new InvalidOperationException("This client core is already running.");
            _socket = bapsConnection;

            var tf = new TaskFactory(_dead.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None,
                TaskScheduler.Current);
            CreateAndLaunchReceiver(tf);
            CreateAndLaunchSender(tf);
        }

        private void CreateAndLaunchReceiver(TaskFactory tf)
        {
            if (_socket == null) throw new NullReferenceException("Tried to launch receiver with a null socket.");

            // TODO(@MattWindsor91): inject these dependencies.
            var decoder = new ClientCommandDecoder(_socket, _dead.Token);
            _receiver = new Receiver(_socket, decoder, _dead.Token);
            SubscribeToReceiver();
            _receiverTask = tf.StartNew(_receiver.Run);
        }

        private void CreateAndLaunchSender(TaskFactory tf)
        {
            _sender = new Sender(_socket, _dead.Token);
            _senderTask = tf.StartNew(_sender.Run);
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = new SystemCommand(SystemOp.End);
            Send(new MessageBuilder(cmd).Add("Normal Termination"));
        }

        /// <summary>
        ///     Cancels and joins the receiver and sender tasks.
        /// </summary>
        private void CancelTasks()
        {
            _dead.CancelAfter(CancelGracePeriodMilliseconds);
            // Force the receive thread to abort FIRST so that we cant receive
            // any messages that need automatic responses
            Join(_receiverTask);
            _receiverTask = null;
            Join(_senderTask);
            _senderTask = null;
        }

        /// <summary>
        ///     Waits for the given task to finish, then disposes it.
        /// </summary>
        /// <param name="task">The task to join.</param>
        private static void Join(Task? task)
        {
            if (task == null) return;
            try
            {
                task.Wait();
            }
            catch (AggregateException a)
            {
                a.Handle(e => e is OperationCanceledException);
            }

            task.Dispose();
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                NotifyServerOfQuit();
                CancelTasks();
                UnsubscribeFromReceiver();

                _socket?.Dispose();
            }

            _disposedValue = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}