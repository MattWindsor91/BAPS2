using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater : IBaseServerUpdater
    {
        IObservable<DirectoryFileAddEventArgs> ObserveDirectoryFileAdd { get; }
        IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare { get; }
    }
}