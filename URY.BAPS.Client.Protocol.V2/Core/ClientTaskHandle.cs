using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     A handle that allows for waiting on the client sender and receiver
    ///     tasks.
    /// </summary>
    public sealed class ClientTaskHandle : IDisposable
    {
        private readonly Task _receiverTask;

        private readonly Task _senderTask;

        /// <summary>
        ///     Creates tasks for the given sender and receiver, launches them,
        ///     and returns a handle that can be used to wait on them.
        /// </summary>
        /// <param name="receiver">The <see cref="Receiver"/> to create a task for.</param>
        /// <param name="sender">The <see cref="Sender"/> to create a task for.</param>
        /// <param name="token">
        ///     The <see cref="CancellationToken"/> that will be used to cancel
        ///     the sender and receiver when their client connection is no
        ///     longer needed.
        /// </param>
        /// <returns>
        ///     A <see cref="ClientTaskHandle"/> that can be used to wait on
        ///     both sender and receiver.
        /// </returns>
        public static ClientTaskHandle CreateAndLaunchTasks(Receiver receiver, Sender sender, CancellationToken token)
        {
            var tf = new TaskFactory(token, TaskCreationOptions.LongRunning, TaskContinuationOptions.None,
                TaskScheduler.Current);
            var receiverTask = tf.StartNew(receiver.Run, token);
            var senderTask = tf.StartNew(() => sender.Run(token), token);

            return new ClientTaskHandle(receiverTask, senderTask);
        }

        public ClientTaskHandle(Task receiverTask, Task senderTask)
        {
            _receiverTask = receiverTask;
            _senderTask = senderTask;
        }

        public void Dispose()
        {
            _receiverTask.Dispose();
            _senderTask.Dispose();
        }

        /// <summary>
        ///     Waits for both receiver and sender (in that order) to shut
        ///     down, silently ignoring any thrown cancellation exceptions.
        /// </summary>
        public void Wait()
        {
            // Force the receive thread to abort FIRST so that we cant receive
            // any messages that need automatic responses.
            WaitOne(_receiverTask);
            WaitOne(_senderTask);
        }

        /// <summary>
        ///     Waits for the given task to finish, silently ignoring
        ///     cancellation. 
        /// </summary>
        /// <param name="task">The task to join.</param>
        private static void WaitOne(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException a)
            {
                a.Handle(e => e is OperationCanceledException);
            }
        }
    }
}
