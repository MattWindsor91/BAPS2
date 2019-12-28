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
        /// <return>
        ///     The subscription produced by attaching, which can either be
        ///     ignored (as the feed will detach and dispose all subscriptions
        ///     when disposed itself), or passed to <see cref="Detach"/> to
        ///     dispose the subscription early.
        /// </return>
        public IDisposable Attach(IObservable<MessageArgsBase> obs)
        {
            var sub = obs.Subscribe(_bridge);
            _subscriptions.Add(sub);
            return sub;
        }

        /// <summary>
        ///     Detaches this updater from the source specified by the given
        ///     subscription (as returned by <see cref="Attach"/>).
        /// </summary>
        /// <param name="subscription">
        ///     The subscription to remove.
        /// </param>
        /// <return>Whether or not <paramref name="subscription"/> was adequately disposed-of.</return>
        public bool Detach(IDisposable subscription)
        {
            return _subscriptions.Remove(subscription);
        }

        /// <summary>
        ///     Detaches this updater from all sources to which it was
        ///     previously attached using <see cref="Attach"/>.
        /// </summary>
        public void DetachAll()
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