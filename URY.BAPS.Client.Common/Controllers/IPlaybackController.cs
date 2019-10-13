using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Interface for controllers that allow players to interact with the
    ///     playback aspects of a BAPS server.
    /// </summary>
    public interface IPlaybackController
    {
        /// <summary>
        ///     An event feed for playback messages affecting this channel.
        /// </summary>
        IPlaybackEventFeed PlaybackUpdater { get; }

        /// <summary>
        ///     Asks the server to set this channel's state to <see cref="state" />.
        /// </summary>
        /// <param name="state">The intended new state of the channel.</param>
        void SetState(PlaybackState state);

        /// <summary>
        ///     Asks the BAPS server to move one of this channel's markers.
        /// </summary>
        /// <param name="type">The type of marker to move.</param>
        /// <param name="newValue">The requested new value.</param>
        void SetMarker(MarkerType type, uint newValue);
    }
}