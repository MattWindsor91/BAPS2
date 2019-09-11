using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Event interface for classes that contain BapsNet playlist event feeds.
    /// </summary>
    public interface IPlaylistEventFeed : IEventFeed
    {
        IObservable<TrackAddArgs> ObserveTrackAdd { get; }
        IObservable<TrackDeleteArgs> ObserveTrackDelete { get; }
        IObservable<TrackMoveArgs> ObserveTrackMove { get; }

        IObservable<PlaylistResetArgs> ObservePlaylistReset { get; }
    }
}