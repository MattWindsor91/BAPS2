using System;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for player view models.
    ///     <para>
    ///         Player view models are a composite of various smaller view
    ///         models, each of which governs a specific part of the player UI.
    ///     </para>
    /// </summary>
    public interface IPlayerViewModel : IDisposable
    {
        /// <summary>
        ///     The sub-view-model for the currently loaded track part of the player.
        /// </summary>
        IPlayerTrackViewModel Track { get; }

        /// <summary>
        ///     The sub-view-model for the transport (play/stop/pause) part of
        ///     the player.
        /// </summary>
        IPlayerTransportViewModel Transport { get; }

        /// <summary>
        ///     The sub-view-model for the marker (position/cue/intro) part of
        ///     the player.
        /// </summary>
        IPlayerMarkerViewModel Markers { get; }
    }
}