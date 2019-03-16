using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BapsNet server playlist updates.
    /// </summary>
    public interface IPlaylistServerUpdater : IBaseServerUpdater
    {
        IObservable<TrackAddEventArgs> ObserveTrackAdd { get; }
        IObservable<TrackDeleteEventArgs> ObserveTrackDelete { get; }
        IObservable<TrackMoveEventArgs> ObserveTrackMove { get; }

        IObservable<PlaylistResetEventArgs> ObservePlaylistReset { get; }
    }
}