using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Basic interface for classes that expose observable feeds for
    ///     BAPS events.
    /// </summary>
    public interface IEventFeed
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