using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    /// <inheritdoc />
    /// <summary>
    ///     Object encapsulating the core features of a BapsNet client.
    /// </summary>
    public class ClientCore : IDisposable
    {
        // TODO(@MattWindsor91): don't hard-code these
        public const ushort NumChannels = 3;
        private const ushort NumDirectories = 3;

        private const int CancelGracePeriodMilliseconds = 500;
        private readonly Authenticator _auth;
        private readonly CancellationTokenSource _dead = new CancellationTokenSource();

        private Receiver _receiver;
        private Task _receiverTask;

        private Sender _sender;
        private Task _senderTask;

        private ClientSocket _socket;
        private readonly ConfigCache _configCache;

        /// <summary>
        ///     The set of controllers used to translate channel requests to
        ///     BapsNet commands.
        /// </summary>
        private readonly Dictionary<uint, ChannelController> _channelControllers = new Dictionary<uint, ChannelController>();

        public ChannelController ControllerFor(ushort channelId)
        {
            if (!_channelControllers.TryGetValue(channelId, out var controller))
                throw new IndexOutOfRangeException($"Invalid channel ID: {channelId}");
            return controller;
        }


        public ClientCore(Authenticator auth, ConfigCache cache)
        {
            _auth = auth;
            _configCache = cache;
        }

        /// <summary>
        ///     A thread-safe queue for outgoing BAPSNet messages.
        /// </summary>
        public BlockingCollection<Message> SendQueue { get; } = new BlockingCollection<Message>();

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
        ///     Event raised when the <see cref="ClientCore" /> has just authenticated.
        /// </summary>
        public event EventHandler Authenticated;

        private void OnAuthenticated()
        {
            Authenticated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Event raised when the <see cref="ClientCore" /> has created a receiver.
        ///     Subscribe to this in order to attach reactions to receiver events.
        /// </summary>
        public event EventHandler<Receiver> ReceiverCreated;

        private void OnReceiverCreated()
        {
            ReceiverCreated?.Invoke(this, _receiver);
        }

        /// <summary>
        ///     Tries to authenticate and launch a BAPS client.
        /// </summary>
        /// <returns>Whether the client successfully launched.</returns>
        public bool Launch()
        {
            OnAboutToAuthenticate();

            var authenticated = Authenticate();
            if (!authenticated) return false;

            SetupChannelControllers();
            OnAuthenticated();

            EnqueueAutoUpdate();
            LaunchTasks();

            return true;
        }

        private void SetupChannelControllers()
        {
            for (ushort i = 0; i < NumChannels; i++)
            {
                _channelControllers.Add(i, new ChannelController(i, SendQueue, _configCache));
            }
        }

        private void LaunchTasks()
        {
            var tf = new TaskFactory(_dead.Token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None,
                TaskScheduler.Current);
            _receiver = new Receiver(_socket, _dead.Token);
            OnReceiverCreated();
            _receiverTask = tf.StartNew(_receiver.Run);
            _sender = new Sender(SendQueue, _dead.Token, _socket);
            _senderTask = tf.StartNew(_sender.Run);
        }

        private bool Authenticate()
        {
            Debug.Assert(_auth != null, "Tried to authenticate with null authenticator");
            _socket = _auth.Run();
            return _socket != null;
        }

        private void EnqueueAutoUpdate()
        {
            // Add the auto-update message onto the queue (chat(2) and general(1))
            var cmd = Command.System | Command.AutoUpdate | (Command) 2 | (Command) 1;
            SendQueue.Add(new Message(cmd));
            for (var i = 0; i < NumDirectories; i++)
            {
                /** Add the refresh folder onto the queue **/
                cmd = Command.System | Command.ListFiles | (Command) i;
                SendQueue.Add(new Message(cmd));
            }
        }

        /// <summary>
        ///     Sends a BAPSNet message telling the server that we've quit.
        /// </summary>
        private void NotifyServerOfQuit()
        {
            const Command cmd = Command.System | Command.End;
            SendQueue.Add(new Message(cmd).Add("Normal Termination"));
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