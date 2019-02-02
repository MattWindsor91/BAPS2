using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from somewhere
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public abstract class EventMessengerAdapterBase
    {
        protected IMessenger Messenger;

        protected EventMessengerAdapterBase(IMessenger messenger)
        {
            Messenger = messenger;
        }
        
        /// <summary>
        ///     Forwards an event's payload as a messenger message.
        /// </summary>
        /// <typeparam name="T">Type of event payload.</typeparam>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">The object payload.</param>
        protected void Relay<T>(object sender, T e)
        {
            Messenger.Send(e);
        }
    }
}