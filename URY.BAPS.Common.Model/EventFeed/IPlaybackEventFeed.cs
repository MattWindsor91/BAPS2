using System;
using URY.BAPS.Common.Model.MessageEvents;

namespace URY.BAPS.Common.Model.EventFeed
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackEventFeed : IEventFeed
    {
        /// <summary>
        ///     Observable that reports when the server reports a change in channel state.
        /// </summary>
        IObservable<PlaybackStateChangeArgs> ObservePlayerState { get; }

        /// <summary>
        ///     Observable that reports when the server reports a change in channel marker.
        /// </summary>
        IObservable<MarkerChangeArgs> ObserveMarker { get; }

        /// <summary>
        ///     Observable that reports when the server reports that a new track has been loaded into the player.
        /// </summary>
        IObservable<TrackLoadArgs> ObserveTrackLoad { get; }
    }
}