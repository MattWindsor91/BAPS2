using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using ReactiveUI;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for view models over a player's markers.
    ///     <para>
    ///         These view models both present and accept changes over a
    ///         single player's position, cue, and intro.
    ///     </para>
    /// </summary>
    public interface IPlayerMarkerViewModel : IDisposable
    {
        #region Main markers

        /// <summary>
        ///     The position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint Position { get; }

        /// <summary>
        ///     The cue position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint CuePosition { get; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        uint IntroPosition { get; }

        /// <summary>
        ///     The duration of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint Duration { get; }

        #endregion Main markers

        #region Derived markers and scales

        /// <summary>
        ///     The amount of milliseconds remaining in the currently loaded item.
        /// </summary>
        uint Remaining { get; }

        double PositionScale { get; }
        double CuePositionScale { get; }
        double IntroPositionScale { get; }

        #endregion Derived markers and scales

        #region Commands

        /// <summary>
        ///     A command that, when fired, asks the server to move the position marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetPosition { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the cue marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetCue { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the intro marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetIntro { get; }

        /// <summary>
        ///     True if, and only if, the various marker set commands are active.
        /// </summary>
        bool CanSetMarkers { get; }

        #endregion Commands
    }
}
