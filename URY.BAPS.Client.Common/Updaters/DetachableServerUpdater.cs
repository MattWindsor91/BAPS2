using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     A variant of <see cref="FilteringServerUpdater"/> that can take
    ///     subscribers before a server updater is available, and persist them
    ///     if the server updater becomes unavailable.
    /// </summary>
    public class DetachableServerUpdater : FilteringServerUpdater

    {
        private Subject<ArgsBase> bridge = new Subject<ArgsBase>();

        private IDisposable? subscription = null;
        
        public DetachableServerUpdater() : base(Observable.Empty<ArgsBase>())
        {
            ObserveMessages = bridge;
        }

        public void Attach(IObservable<ArgsBase> obs)
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