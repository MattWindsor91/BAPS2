using System;

namespace URY.BAPS.Common.Infrastructure
{
    /// <summary>
    ///     Base interface of message-level BAPS connections.
    /// </summary>
    public interface IMessageConnection : IDisposable
    {
        // TODO(@MattWindsor91): move things from the V2 message connection interface here eventually.
       
        /// <summary>
        ///     Whether the connection object is actually connected to anything.
        /// </summary>
        bool IsConnected { get; }
    }
}