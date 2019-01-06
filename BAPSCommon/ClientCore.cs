using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BAPSCommon
{
    /// <summary>
    /// Object encapsulating the core features of a BAPSnet client.
    /// </summary>
    public class ClientCore : IDisposable
    {
        private Authenticator _auth = null;
        private CancellationTokenSource _dead = new CancellationTokenSource();

        private Sender _sender = null;
        private Task _senderTask = null;

        private Receiver _receiver = null;
        private Task _receiverTask = null;

        private ClientSocket _socket = null;

        /// <summary>
        /// A thread-safe queue for outgoing BAPSnet messages.
        /// </summary>
        public BlockingCollection<Message> SendQueue { get; } = new BlockingCollection<Message>();

        public ClientCore(Func<Authenticator.Response> loginCallback)
        {
            _auth = new Authenticator(loginCallback, _dead.Token);
        }

        /// <summary>
        /// Event raised when the <see cref="ClientCore"/> has just authenticated.
        /// </summary>
        public event EventHandler Authenticated;
        private void OnAuthenticated() => Authenticated?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Event raised when the <see cref="ClientCore"/> has created a receiver.
        /// Subscribe to this in order to attach reactions to receiver events.
        /// </summary>
        public event EventHandler<Receiver> ReceiverCreated;
        private void OnReceiverCreated() => ReceiverCreated?.Invoke(this, _receiver);
        
        /// <summary>
        /// Tries to authenticate and launch a BAPS client.
        /// </summary>
        /// <returns>Whether the client successfully launched.</returns>
        public bool Launch()
        {
            var authenticated = Authenticate();
            if (!authenticated) return false;
            OnAuthenticated();

            EnqueueAutoUpdate();

            var tf = new TaskFactory(_dead.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, TaskScheduler.Current);
            _receiver = new Receiver(_socket, _dead.Token);
            OnReceiverCreated();
            _receiverTask = tf.StartNew(_receiver.Run);
            _sender = new Sender(SendQueue, _dead.Token, _socket);
            _senderTask = tf.StartNew(_sender.Run);

            return true;
        }

        private bool Authenticate()
        {
            Debug.Assert(_auth != null, "Tried to authenticate with null authenticator");
            _socket = _auth.Run();
            return _socket != null;
        }

        private void EnqueueAutoUpdate()
        {
            // Add the autoupdate message onto the queue (chat(2) and general(1))
            Command cmd = Command.SYSTEM | Command.AUTOUPDATE | (Command)2 | (Command)1;
            SendQueue.Add(new Message(cmd));
            for (int i = 0; i < 3; i++)
            {
                /** Add the refresh folder onto the queue **/
                cmd = Command.SYSTEM | Command.LISTFILES | (Command)i;
                SendQueue.Add(new Message(cmd));
            }
        }

        /// <summary>
        /// Sends a BAPSnet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            var cmd = Command.SYSTEM | Command.END;
            SendQueue.Add(new Message(cmd).Add("Normal Termination"));
        }

        private const int CancelGracePeriodMsec = 500;

        /// <summary>
        /// Cancels and joins the receiver and sender tasks.
        /// </summary>
        private void CancelTasks()
        {
            _dead.CancelAfter(CancelGracePeriodMsec);
            // Force the receive thread to abort FIRST so that we cant receive
            // any messages that need automatic responses
            Join(_receiverTask);
            _receiverTask = null;
            Join(_senderTask);
            _senderTask = null;
        }

        /// <summary>
        /// Waits for the given task to finish, then disposes it.
        /// </summary>
        /// <param name="task">The task to join.</param>
        private void Join(Task task)
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
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
            {
                NotifyServerOfQuit();
                CancelTasks();

                _socket?.Dispose();
            }

            disposedValue = true;
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
