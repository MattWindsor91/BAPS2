using System.Collections.Concurrent;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon.Controllers
{
    /// <summary>
    ///     Base class of controllers that perform actions by sending BapsNet
    ///     messages.
    /// </summary>
    public abstract class BapsNetControllerBase
    {
        private readonly BlockingCollection<Message> _messageQueue;

        /// <summary>
        ///     Base constructor for BapsNet controllers.
        /// </summary>
        /// <param name="core">The client core to use to send messages.</param>
        protected BapsNetControllerBase(ClientCore core)
        {
            _messageQueue = core.SendQueue;
        }

        /// <summary>
        ///     Sends a BapsNet message through this controller's queue.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected void Send(Message message)
        {
            _messageQueue.Add(message);
        }
    }
}