using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    public interface IDatabaseEventFeed : IEventFeed
    {
        IObservable<LibraryResultArgs> ObserveLibraryResult { get; }
        IObservable<ListingResultArgs> ObserveListingResult { get; }
        IObservable<ShowResultArgs> ObserveShowResult { get; }
    }
}