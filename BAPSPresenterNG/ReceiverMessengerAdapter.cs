using System;
using BAPSClientCommon;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from a <see cref="Receiver" />
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public class ReceiverMessengerAdapter : EventMessengerAdapterBase
    {
        [NotNull] private readonly Receiver _receiver;

        /// <summary>
        ///     Constructs a <see cref="ReceiverMessengerAdapter" />.
        ///     <para>
        ///         This constructor registers the appropriate event handlers
        ///         on <paramref name="receiver" />.
        ///     </para>
        /// </summary>
        /// <param name="receiver">The receiver to listen to.</param>
        /// <param name="messenger">The messenger bus to forward onto.</param>
        public ReceiverMessengerAdapter([CanBeNull] Receiver receiver, [CanBeNull] IMessenger messenger) : base(messenger)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            Register();
        }

        /// <summary>
        ///     Attaches event handlers to the receiver that put the event's
        ///     payload on the messenger bus.
        /// </summary>
        private void Register()
        {
            // Each event has a distinct type, and therefore each of these
            // uses of Relay is registering a separate message, despite
            // appearances.

            _receiver.ChannelState += Relay;
            _receiver.ChannelMarker += Relay;
            _receiver.TrackLoad += Relay;

            _receiver.ItemAdd += Relay;
            _receiver.ItemMove += Relay;
            _receiver.ItemDelete += Relay;
            _receiver.ResetPlaylist += Relay;

            _receiver.DirectoryFileAdd += Relay;
            _receiver.DirectoryPrepare += Relay;
        }
    }
}