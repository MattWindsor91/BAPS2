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
    /// <summary>
    ///     Object encapsulating the core features of a BapsNet client.
    /// </summary>
    public class ClientCore : IClientCore
    {
        private const int CountPrefetchTimeoutMilliseconds = 500;
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

        /// <summary>
        ///     A thread-safe queue for outgoing BAPSNet messages.
        /// </summary>
        [ItemNotNull, NotNull]
        private BlockingCollection<Message> SendQueue { get; } = new BlockingCollection<Message>();

        /// <summary>
        ///     Sends a BapsNet message asynchronously through this client's
        ///     BapsNet connection.
        /// </summary>
        /// <param name="message">The message to send.  If null, nothing is sent.</param>
        public void SendAsync(Message message)
        {
            if (message != null) SendQueue.Add(message);
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

            LaunchTasks();

            var numChannels = PrefetchCount(OptionKey.ChannelCount);
            var numDirectories = PrefetchCount(OptionKey.DirectoryCount);

            OnAboutToAutoUpdate((numChannels, numDirectories));
            EnqueueAutoUpdate(numDirectories);

            return true;
        }

        /// <summary>
        ///     Synchronously polls the BAPS server for an integer config
        ///     setting (eg, a channel or directory count).
        ///     <para>
        ///         This is intended as a workaround for certain BAPS server
        ///         versions, whose auto-update runs require up-front directory
        ///         counts, and send channel information before sending the
        ///         channel count itself.
        ///     </para>
        /// </summary>
        /// <param name="key">The key of the count option to poll.</param>
        /// <returns>The value (or 0 if the prefetch timed out).</returns>
        private int PrefetchCount(OptionKey key)
        {
            var count = 0;
            var optionId = (uint) key;

            using (var wait = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                void Waiter(object sender, Updates.ConfigSettingArgs args)
                {
                    if (args.OptionId != optionId) return;
                    if (args.Value is int v) count = v;
                    _receiver.ConfigSetting -= Waiter;
                    wait.Set();
                }

                _receiver.ConfigSetting += Waiter;
                SendQueue.Add(new Message(Command.Config | Command.GetConfigSetting).Add(optionId));
                wait.WaitOne(CountPrefetchTimeoutMilliseconds);
            }

            return count;
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

        private void EnqueueAutoUpdate(int numDirectories)
        {
            // Add the auto-update message onto the queue (chat(2) and general(1))
            var cmd = Command.System | Command.AutoUpdate | (Command) 2 | (Command) 1;
            SendQueue.Add(new Message(cmd));
            for (var i = 0; i < numDirectories; i++)
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