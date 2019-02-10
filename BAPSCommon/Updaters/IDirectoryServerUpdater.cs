using System;
using BAPSClientCommon.Events;

namespace BAPSClientCommon.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server directory updates.
    /// </summary>
    public interface IDirectoryServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.DirectoryFileAddEventArgs> ObserveDirectoryFileAdd { get; }
        IObservable<Updates.DirectoryPrepareEventArgs> ObserveDirectoryPrepare { get; }
    }
}