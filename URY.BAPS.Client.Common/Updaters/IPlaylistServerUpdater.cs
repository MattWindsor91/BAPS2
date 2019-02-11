using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BapsNet server playlist updates.
    /// </summary>
    public interface IPlaylistServerUpdater : IBaseServerUpdater
    {
        IObservable<Updates.TrackAddEventArgs> ObserveTrackAdd { get; }
        IObservable<Updates.TrackDeleteEventArgs> ObserveTrackDelete { get; }
        IObservable<Updates.TrackMoveEventArgs> ObserveTrackMove { get; }

        IObservable<Updates.PlaylistResetEventArgs> ObservePlaylistReset { get; }
    }
}