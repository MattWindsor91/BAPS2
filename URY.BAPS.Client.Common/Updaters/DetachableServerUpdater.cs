using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     A variant of <see cref="FilteringServerUpdater"/> that can take
    ///     subscribers before a server updater is available, and persist them
    ///     if the server updater becomes unavailable.
    /// </summary>
    public class DetachableServerUpdater : FilteringServerUpdater

    {
        private Subject<MessageArgsBase> bridge = new Subject<MessageArgsBase>();

        private IDisposable? subscription = null;
        
        public DetachableServerUpdater() : base(Observable.Empty<MessageArgsBase>())
        {
            ObserveMessages = bridge;
        }

        public void Attach(IObservable<MessageArgsBase> obs)
        {
            subscription ??= obs.Subscribe(bridge);
        }

        public void Detach()
        {
            subscription?.Dispose();
            subscription = null;
        }
    }
}