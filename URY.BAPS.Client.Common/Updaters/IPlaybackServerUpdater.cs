using System;
using URY.BAPS.Client.Common.Events;

namespace URY.BAPS.Client.Common.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server reports a change in channel state.
        /// </summary>
        IObservable<PlayerStateEventArgs> ObservePlayerState { get; }

        /// <summary>
        ///     Observable that reports when the server reports a change in channel marker.
        /// </summary>
        IObservable<MarkerEventArgs> ObserveMarker { get; }

        /// <summary>
        ///     Observable that reports when the server reports that a new track has been loaded into the player.
        /// </summary>
        IObservable<TrackLoadEventArgs> ObserveTrackLoad { get; }
    }
}