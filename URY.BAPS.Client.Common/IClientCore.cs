using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Interface for objects encapsulating the core features of a BapsNet client.
    /// </summary>
    public interface IClientCore : IDisposable, IServerUpdater
    {
        /// <summary>
        ///     Sends a BapsNet message asynchronously through this client's
        ///     BapsNet connection.
        /// </summary>
        /// <param name="message">The message to send.  If null, nothing is sent.</param>
        void SendAsync([CanBeNull] Message message);

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