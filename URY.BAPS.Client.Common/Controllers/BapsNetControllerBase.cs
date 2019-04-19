using System;
using JetBrains.Annotations;
using URY.BAPS.Protocol.V2.Messages;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Base class of controllers that perform actions by sending BapsNet
    ///     messages.
    /// </summary>
    public abstract class BapsNetControllerBase
    {
        [NotNull] protected readonly IClientCore Core;

        /// <summary>
        ///     Base constructor for BapsNet controllers.
        /// </summary>
        /// <param name="core">The client core to use to send messages.</param>
        protected BapsNetControllerBase([CanBeNull] IClientCore core)
        {
            Core = core ?? throw new ArgumentNullException(nameof(core));
        }

        /// <summary>
        ///     Sends a BapsNet message through this controller's queue.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected void SendAsync([CanBeNull] Message message)
        {
            Core.SendAsync(message);
        }
    }
}