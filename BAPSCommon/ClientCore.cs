using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.ServerConfig;
using JetBrains.Annotations;

namespace BAPSClientCommon
{
    /// <inheritdoc />
    /// <summary>
    ///     Object encapsulating the core features of a BapsNet client.
    /// </summary>
    public partial class ClientCore : IClientCore
    {
        private const int CancelGracePeriodMilliseconds = 500;
        private readonly Authenticator _auth;

        private readonly CancellationTokenSource _dead = new CancellationTokenSource();

        private Receiver _receiver;
        private Task _receiverTask;

        private Sender _sender;
        private Task _senderTask;

        private ClientSocket _socket;


        public ClientCore([NotNull] Authenticator auth)
        {
            _auth = auth;
            _auth.Token = _dead.Token;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="message">The message to send.  If null, nothing is sent.</param>
        public void SendAsync(Message message)
        {
            if (message != null) _sender?.SendAsync(message);
        }
        
        /// <summary>
        ///     Event raised just before authentication.
        ///     Subscribe to this to install any event handlers needed for the authenticator.
        /// </summary>
        public event EventHandler<Authenticator> AboutToAuthenticate;

        private void OnAboutToAuthenticate()
        {
            AboutToAuthenticate?.Invoke(this, _auth);
        }

        /// <summary>
        ///     Event raised when the <see cref="ClientCore" /> is about to start
        ///     auto-updating.
        ///     <para>
        ///         This event supplies any pre-fetched counts in advance of the auto-update.
        ///     </para>
        /// </summary>
        public event EventHandler<(int numChannelsPrefetch, int numDirectoriesPrefetch)> AboutToAutoUpdate;

        /// <summary>
        ///     Tries to authenticate and launch a BAPS client.
        /// </summary>
        /// <returns>Whether the client successfully launched.</returns>
        public bool Launch()
        {
            OnAboutToAuthenticate();

            var authenticated = Authenticate();
            if (!authenticated) return false;

            LaunchTasks();

            return true;
        }
        
        private void LaunchTasks()
        {
            var tf = new TaskFactory(_dead.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None,
                TaskScheduler.Current);
            CreateAndLaunchReceiver(tf);
            CreateAndLaunchSender(tf);
        }

        private void CreateAndLaunchReceiver(TaskFactory tf)
        {
            _receiver = new Receiver(_socket, _dead.Token);
            AttachReceiverEvents();
            _receiverTask = tf.StartNew(_receiver.Run);
        }

        private void CreateAndLaunchSender(TaskFactory tf)
        {
            _sender = new Sender(_dead.Token, _socket);
            _senderTask = tf.StartNew(_sender.Run);
        }

        private bool Authenticate()
        {
            Debug.Assert(_auth != null, "Tried to authenticate with null authenticator");
            _socket = _auth.Run();
            return _socket != null;
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            const Command cmd = Command.System | Command.End;
            SendAsync(new Message(cmd).Add("Normal Termination"));
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
        private static void Join(Task task)
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

        protected virtual void OnAboutToAutoUpdate((int numChannelsPrefetch, int numDirectoriesPrefetch) e)
        {
            AboutToAutoUpdate?.Invoke(this, e);
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