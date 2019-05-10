using System;
using URY.BAPS.Model.MessageEvents;

namespace URY.BAPS.Client.Common.Updaters
{
    public interface IDatabaseServerUpdater : IBaseServerUpdater
    {
        IObservable<LibraryResultArgs> ObserveLibraryResult { get; }
        IObservable<ListingResultArgs> ObserveListingResult { get; }
        IObservable<ShowResultArgs> ObserveShowResult { get; }
    }
}