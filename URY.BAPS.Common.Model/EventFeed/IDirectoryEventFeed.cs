using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Observable interface for classes that contain BapsNet directory event feeds.
    /// </summary>
    public interface IDirectoryEventFeed : IEventFeed
    {
        /// <summary>
        ///     An observable that triggers whenever a file is added to a
        ///     directory.
        /// </summary>
        IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd { get; }

        /// <summary>
        ///     An observable that triggers whenever a directory resets,
        ///     clearing its contents and possibly changing its name.
        /// </summary>
        IObservable<DirectoryPrepareArgs> ObserveDirectoryPrepare { get; }
    }
}