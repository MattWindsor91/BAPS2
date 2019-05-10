using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Protocol.V2.Encode;

namespace URY.BAPS.Client.Common
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
        ///     Event raised just before authentication.
        ///     Subscribe to this to install any event handlers needed for the authenticator.
        /// </summary>
        event EventHandler<Authenticator> AboutToAuthenticate;

        /// <summary>
        ///     Tries to authenticate and launch a BAPS client.
        /// </summary>
        /// <returns>Whether the client successfully launched.</returns>
        bool Launch();
    }
}