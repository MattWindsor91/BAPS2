using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     A variant of <see cref="FilteringServerUpdater" /> that can take
    ///     subscribers before a server updater is available, and persist them
    ///     if the server updater becomes unavailable.
    /// </summary>
    public class DetachableServerUpdater : FilteringServerUpdater

    {
        private readonly Subject<MessageArgsBase> _bridge = new Subject<MessageArgsBase>();

        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public DetachableServerUpdater() : base(Observable.Empty<MessageArgsBase>())
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
            foreach (var subscription in _subscriptions) subscription.Dispose();
            _subscriptions.Clear();
        }
    }
}