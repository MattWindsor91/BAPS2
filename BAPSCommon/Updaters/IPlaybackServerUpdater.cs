using System;
using BAPSClientCommon.Events;

namespace BAPSClientCommon.Updaters
{
    /// <summary>
    ///     Observable interface for classes that send BapsNet server playback updates.
    /// </summary>
    public interface IPlaybackServerUpdater : IBaseServerUpdater
    {
        /// <summary>
        ///     Observable that reports when the server reports a change in channel state.
        /// </summary>
        IObservable<Updates.PlayerStateEventArgs> ObservePlayerState { get; }

        /// <summary>
        ///     Observable that reports when the server reports a change in channel marker.
        /// </summary>
        IObservable<Updates.MarkerEventArgs> ObserveMarker { get; }

        /// <summary>
        ///     Observable that reports when the server reports that a new track has been loaded into the player.
        /// </summary>
        IObservable<Updates.TrackLoadEventArgs> ObserveTrackLoad { get; }
    }
}