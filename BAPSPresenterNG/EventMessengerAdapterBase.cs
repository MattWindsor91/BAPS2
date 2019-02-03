using System;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     An object that listens to events from somewhere
    ///     and forwards them onto the messenger bus.
    /// </summary>
    public abstract class EventMessengerAdapterBase
    {
        [NotNull] private readonly IMessenger _messenger;

        protected EventMessengerAdapterBase([CanBeNull] IMessenger messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }

        /// <summary>
        ///     Forwards an event's payload as a messenger message.
        /// </summary>
        /// <typeparam name="T">Type of event payload.</typeparam>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">The object payload.</param>
        protected void Relay<T>([CanBeNull] object sender, T e)
        {
            _messenger.Send(e);
        }
    }
}