using System;
using System.Reactive;
using ReactiveUI;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Interface for player view models.
    /// </summary>
    public interface IPlayerViewModel : IDisposable
    {
        /// <summary>
        ///     The expected start time of the currently loaded item (if any).
        /// </summary>
        uint StartTime { get; set; }

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        ITrack LoadedTrack { get; }

        /// <summary>
        ///     True provided that there is a currently loaded item, and it is an audio track.
        /// </summary>
        bool HasLoadedAudioTrack { get; }

        /// <summary>
        ///     Whether this channel is playing, according to the server.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        ///     Whether this channel is paused, according to the server.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        ///     Whether this channel is stopped, according to the server.
        /// </summary>
        bool IsStopped { get; }

        /// <summary>
        ///     The position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint Position { get; }

        /// <summary>
        ///     The duration of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint Duration { get; }

        /// <summary>
        ///     The amount of milliseconds remaining in the currently loaded item.
        /// </summary>
        uint Remaining { get; }

        /// <summary>
        ///     The cue position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint CuePosition { get; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        uint IntroPosition { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Play { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Pause { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        ReactiveCommand<Unit, Unit> Stop { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the cue marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetCue { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the position marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetPosition { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the intro marker to the given position.
        /// </summary>
        ReactiveCommand<uint, Unit> SetIntro { get; }
    }
}