using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     A variant of <see cref="FilteringEventFeed" /> that can take
    ///     subscribers before a server updater is available, and persist them
    ///     if the server updater becomes unavailable.
    /// </summary>
    public class DetachableEventFeed : FilteringEventFeed, IDisposable

    {
        private readonly Subject<MessageArgsBase> _bridge = new Subject<MessageArgsBase>();

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public DetachableEventFeed() : base(Observable.Empty<MessageArgsBase>())
        {
            ObserveMessages = _bridge;
        }

        /// <summary>
        ///     Attaches this updater to a downstream source of server updates.
        /// </summary>
        /// <param name="obs">
        ///     The observable source of server updates.
        /// </param>
        public void Attach(IObservable<MessageArgsBase> obs)
        {
            _subscriptions.Add(obs.Subscribe(_bridge));
        }

        /// <summary>
        ///     Detaches this updater from all sources to which it was
        ///     previously attached using <see cref="Attach"/>.
        /// </summary>
        public void Detach()
        {
            _subscriptions.Clear();
        }

        public void Dispose()
        {
            _bridge?.Dispose();
            _subscriptions?.Dispose();
        }
    }
}