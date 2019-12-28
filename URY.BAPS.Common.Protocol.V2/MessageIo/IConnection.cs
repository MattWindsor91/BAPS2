using System;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Encode;

namespace URY.BAPS.Common.Protocol.V2.MessageIo
{
    /// <summary>
    ///     Interface of message-level BapsNet connections.
    /// </summary>
    public interface IConnection : IDisposable
    {
        IFullEventFeed EventFeed { get; }

        /// <summary>
        ///     Sends a message to the BapsNet server.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        void Send(MessageBuilder? messageBuilder);
    }
}