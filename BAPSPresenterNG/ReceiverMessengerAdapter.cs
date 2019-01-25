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
        ///     Attaches event handlers to the receiver that put the event's
        ///     payload on the messenger bus.
        /// </summary>
        public void Register()
        {
            _receiver.ChannelState += (sender, e) => _messenger.Send(e);

            _receiver.ItemAdd += (sender, e) => _messenger.Send(e);
            _receiver.ItemMove += (sender, e) => _messenger.Send(e);
            _receiver.ItemDelete += (sender, e) => _messenger.Send(e);
            _receiver.ResetPlaylist += (sender, e) => _messenger.Send(e);

            _receiver.DirectoryFileAdd += (sender, e) => _messenger.Send(e);
            _receiver.DirectoryPrepare += (sender, e) => _messenger.Send(e);
        }
    }
}