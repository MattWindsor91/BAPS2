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

        public ReceiverMessengerAdapter(Receiver r, IMessenger m)
        {
            receiver = r;
            messenger = m;
        }

        public void Register()
        {
            receiver.ItemAdd += (sender, e) => messenger.Send(e);
            receiver.ItemMove += (sender, e) => messenger.Send(e);
            receiver.ItemDelete += (sender, e) => messenger.Send(e);
            receiver.ResetPlaylist += (sender, e) => messenger.Send(e);
        }
    }
}
