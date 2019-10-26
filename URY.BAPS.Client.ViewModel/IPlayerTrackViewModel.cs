using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for player track view models.
    ///     <para>
    ///         This part of the player view model set-up mostly just manages
    ///         the currently loaded track for a player.
    ///     </para>
    /// </summary>
    public interface IPlayerTrackViewModel
    {
        /// <summary>
        ///     The most recently loaded track.
        /// </summary>
        ITrack LoadedTrack { get; }
    }
}