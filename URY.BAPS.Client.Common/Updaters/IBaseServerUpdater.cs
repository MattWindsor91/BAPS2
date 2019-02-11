using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Observable interface common to all server update classes.
    /// </summary>
    public interface IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that notifies subscribers that a particular number of items is en route.
        /// </summary>
        IObservable<Updates.CountEventArgs> ObserveIncomingCount { get; }
    }
}