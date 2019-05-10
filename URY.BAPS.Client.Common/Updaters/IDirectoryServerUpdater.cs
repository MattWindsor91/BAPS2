using System;
using URY.BAPS.Model.MessageEvents;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater : IBaseServerUpdater
    {
        IObservable<DirectoryFileAddArgs> ObserveDirectoryFileAdd { get; }
        IObservable<DirectoryPrepareEventArgs> ObserveDirectoryPrepare { get; }
    }
}