using System;
using URY.BAPS.Common.Model.MessageEvents;

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
        IObservable<CountArgs> ObserveIncomingCount { get; }

        /// <summary>
        ///     Observable that notifies subscribers that an unknown command was received.
        /// </summary>
        IObservable<UnknownCommandArgs> ObserveUnknownCommand { get; }
    }
}