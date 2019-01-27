using BAPSClientCommon;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from a <see cref="Receiver" />
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public class ReceiverMessengerAdapter
    {
        private readonly IMessenger _messenger;
        private readonly Receiver _receiver;

        /// <summary>
        ///     Constructs a <see cref="ReceiverMessengerAdapter" />.
        ///     <para>
        ///         This constructor doesn't register event handlers; use
        ///         <see cref="Register" /> to do so.
        ///     </para>
        /// </summary>
        /// <param name="r">The receiver to listen to.</param>
        /// <param name="m">The messenger bus to forward onto.</param>
        public ReceiverMessengerAdapter(Receiver r, IMessenger m)
        {
            _receiver = r;
            _messenger = m;
        }

        /// <summary>
        ///     Forwards an event's payload as a messenger message.
        /// </summary>
        /// <typeparam name="T">Type of event payload.</typeparam>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">The object payload.</param>
        private void Relay<T>(object sender, T e)
        {
            _messenger.Send(e);
        }

        /// <summary>
        ///     Attaches event handlers to the receiver that put the event's
        ///     payload on the messenger bus.
        /// </summary>
        public void Register()
        {
            _receiver.ChannelState += Relay;
            _receiver.ChannelMarker += Relay; 

            _receiver.ItemAdd += Relay;
            _receiver.ItemMove += Relay;
            _receiver.ItemDelete += Relay;
            _receiver.ResetPlaylist += Relay;

            _receiver.DirectoryFileAdd += Relay;
            _receiver.DirectoryPrepare += Relay;
        }
    }
}