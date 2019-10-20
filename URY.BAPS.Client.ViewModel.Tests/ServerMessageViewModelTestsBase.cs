using System;
using System.Reactive.Linq;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.ViewModel.Tests
{
    /// <summary>
    ///     Abstract base class for test sets that need to pump server events
    ///     into a view model.
    /// </summary>
    public abstract class ServerMessageViewModelTestsBase
    {
        protected ServerMessageViewModelTestsBase()
        {
            var messages =
                from ev in Observable.FromEventPattern<MessageArgsBase>(x => MessageEvent += x, x => MessageEvent -= x)
                select ev.EventArgs;
            Events = new FilteringEventFeed(messages);
        }

        /// <summary>
        ///     An event feed that dispatches messages sent by
        ///     <see cref="SendMessage" />.
        /// </summary>
        protected IFullEventFeed Events { get; }

        private event EventHandler<MessageArgsBase>? MessageEvent;

        /// <summary>
        ///     Sends a message through <see cref="Events" />.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected void SendMessage(MessageArgsBase message)
        {
            MessageEvent?.Invoke(this, message);
        }
    }
}