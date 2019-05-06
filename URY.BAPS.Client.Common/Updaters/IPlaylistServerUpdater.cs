using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Event interface for classes that send BapsNet server playlist updates.
    /// </summary>
    public interface IPlaylistServerUpdater : IBaseServerUpdater
    {
        IObservable<TrackAddArgs> ObserveTrackAdd { get; }
        IObservable<TrackDeleteArgs> ObserveTrackDelete { get; }
        IObservable<TrackMoveArgs> ObserveTrackMove { get; }

        IObservable<PlaylistResetArgs> ObservePlaylistReset { get; }
    }
}