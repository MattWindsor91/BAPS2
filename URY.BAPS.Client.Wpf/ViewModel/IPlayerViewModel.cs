using System;
using GalaSoft.MvvmLight.CommandWpf;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public interface IPlayerViewModel : IDisposable
    {
        /// <summary>
        ///     The expected start time of the currently loaded item (if any).
        /// </summary>
        uint StartTime { get; set; }

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        ITrack LoadedTrack { get; set; }

        /// <summary>
        ///     True provided that there is a currently loaded item, and it is an audio track.
        /// </summary>
        bool HasLoadedAudioTrack { get; }

        /// <summary>
        ///     Whether this channel is playing, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to play, send
        ///         <see cref="PlayCommand" />.
        ///     </para>
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        ///     Whether this channel is paused, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to pause, send
        ///         <see cref="PauseCommand" />.
        ///     </para>
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        ///     Whether this channel is stopped, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to stop, send
        ///         <see cref="StopCommand" />.
        ///     </para>
        /// </summary>
        bool IsStopped { get; }

        /// <summary>
        ///     The position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        uint Position { get; set; }

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
        uint CuePosition { get; set; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        uint IntroPosition { get; set; }

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        RelayCommand PlayCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        RelayCommand PauseCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        RelayCommand StopCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the cue marker to the given position.
        /// </summary>
        RelayCommand<uint> SetCueCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the position marker to the given position.
        /// </summary>
        RelayCommand<uint> SetPositionCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to move the intro marker to the given position.
        /// </summary>
        RelayCommand<uint> SetIntroCommand { get; }
    }
}