using BAPSCommon;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG
{
    /// <summary>
    /// An object that listens to events from a <see cref="BAPSCommon.Receiver"/>
    /// and forwards them onto the MVVMLight messenger bus.
    /// </summary>
    public class ReceiverMessengerAdapter
    {
        private readonly Receiver receiver;
        private readonly IMessenger messenger;

        /// <summary>
        /// Constructs a <see cref="ReceiverMessengerAdapter"/>.
        /// <para>
        /// This constructor doesn't register event handlers; use
        /// <see cref="Register"/> to do so.</para>
        /// </summary>
        /// <param name="r">The receiver to listen to.</param>
        /// <param name="m">The messenger bus to forward onto.</param>
        public ReceiverMessengerAdapter(Receiver r, IMessenger m)
        {
            receiver = r;
            messenger = m;
        }

        /// <summary>
        /// Attaches event handlers to the receiver that put the event's
        /// payload on the messenger bus.
        /// </summary>
        public void Register()
        {
            receiver.ChannelState += (sender, e) => messenger.Send(e);

            receiver.ItemAdd += (sender, e) => messenger.Send(e);
            receiver.ItemMove += (sender, e) => messenger.Send(e);
            receiver.ItemDelete += (sender, e) => messenger.Send(e);
            receiver.ResetPlaylist += (sender, e) => messenger.Send(e);

            receiver.DirectoryFileAdd += (sender, e) => messenger.Send(e);
            receiver.DirectoryPrepare += (sender, e) => messenger.Send(e);
        }
    }
}
