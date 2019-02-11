using GalaSoft.MvvmLight.CommandWpf;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public interface IPlayerViewModel
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
        ///     The position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        double PositionScale { get; }

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
        ///     The cue position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        double CuePositionScale { get; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        uint IntroPosition { get; set; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        double IntroPositionScale { get; }

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
    }
}