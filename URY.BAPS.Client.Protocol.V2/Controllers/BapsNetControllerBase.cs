using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.MessageIo;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Base class of controllers that perform actions by sending BapsNet
    ///     messages.
    /// </summary>
    public abstract class BapsNetControllerBase
    {
        [NotNull] protected readonly DetachableConnection ConnectionManager;

        /// <summary>
        ///     Base constructor for BapsNet controllers.
        /// </summary>
        /// <param name="core">The client core to use to send messages.</param>
        protected BapsNetControllerBase(DetachableConnection? core)
        {
            ConnectionManager = core ?? throw new ArgumentNullException(nameof(core));
        }

        /// <summary>
        ///     Sends a BapsNet message through this controller's queue.
        /// </summary>
        /// <param name="messageBuilder">The message to send.</param>
        protected void Send(MessageBuilder? messageBuilder)
        {
            ConnectionManager.Send(messageBuilder);
        }

        /// <summary>
        ///     Sends a BapsNet message containing one command and no arguments through this controller's queue.
        /// </summary>
        /// <param name="command">The command to send (as a no-argument message).</param>
        protected void Send(ICommand? command)
        {
            if (command != null) Send(new MessageBuilder(command));
        }
    }
}