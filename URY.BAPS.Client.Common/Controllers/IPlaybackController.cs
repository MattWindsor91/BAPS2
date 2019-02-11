using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.Updaters;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Interface for controllers that allow players to interact with the playback aspects of a BAPS server.
    /// </summary>
    public interface IPlaybackController
    {
        /// <summary>
        ///     An event interface that broadcasts playback server updates.
        ///     <para>
        ///         Note that the updates may include other channels; anything subscribing to this interface
        ///         must check incoming events to see if they affect the right channel.
        ///     </para>
        /// </summary>
        IPlaybackServerUpdater PlaybackUpdater { get; }

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