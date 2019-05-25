using System;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Core
{
    /// <summary>
    ///     Interface for objects encapsulating the core features of a BapsNet client.
    /// </summary>
    public interface IClientCore : IDisposable
    {
        /// <summary>
        ///     Gets a server updater instance connected to this client core.
        /// </summary>
        IServerUpdater Updater { get; }

        /// <summary>
        ///     Sends a BapsNet message asynchronously through this client's
        ///     BapsNet connection.
        /// </summary>
        /// <param name="messageBuilder">The message to send.  If null, nothing is sent.</param>
        void Send(MessageBuilder? messageBuilder);

        /// <summary>
        ///     Attaches an authenticated BapsNet connection to this client
        ///     core.
        /// </summary>
        /// <param name="bapsConnection">
        ///     The connection to attach.
        /// </param>
        void Launch(TcpConnection bapsConnection);
    }
}