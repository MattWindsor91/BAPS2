using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Observable interface for classes that contain BapsNet directory event feeds.
    /// </summary>
    public interface IDirectoryEventFeed : IEventFeed
    {
        IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd { get; }
        IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare { get; }
    }
}