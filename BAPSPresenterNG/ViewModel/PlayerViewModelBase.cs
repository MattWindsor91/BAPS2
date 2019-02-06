using BAPSClientCommon.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;

namespace BAPSPresenterNG.ViewModel
{
    public abstract class PlayerViewModelBase : ViewModelBase, IPlayerViewModel
    {
        [CanBeNull] private RelayCommand _pauseCommand;
        [CanBeNull] private RelayCommand _playCommand;
        [CanBeNull] private RelayCommand _stopCommand;
        protected abstract PlaybackState State { get; set; }

        public abstract uint StartTime { get; set; }
        public abstract ITrack LoadedTrack { get; set; }

        public bool IsPlaying => State == PlaybackState.Playing;

        public bool IsPaused => State == PlaybackState.Paused;

        public bool IsStopped => State == PlaybackState.Stopped;

        public abstract uint Position { get; set; }

        public double PositionScale => (double) Position / Duration;

        public uint Duration => LoadedTrack?.Duration ?? 0;

        public uint Remaining => Duration - Position;

        public abstract uint CuePosition { get; set; }

        public double CuePositionScale => (double) CuePosition / Duration;

        public abstract uint IntroPosition { get; set; }

        /// <summary>
        ///     The intro position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        public double IntroPositionScale => (double) IntroPosition / Duration;

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        [NotNull]
        public virtual RelayCommand PlayCommand => _playCommand
                                                    ?? (_playCommand = new RelayCommand(
                                                        RequestPlay,
                                                        CanRequestPlay));

        protected abstract void RequestPlay();
        
        /// <summary>
        ///     Whether it is ok to ask the server to start playing on this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="PlayCommand" /> can fire.</returns>
        protected abstract bool CanRequestPlay();

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        [NotNull]
        public virtual RelayCommand PauseCommand => _pauseCommand
                                                     ?? (_pauseCommand = new RelayCommand(
                                                         RequestPause,
                                                         CanRequestPause));

        protected abstract void RequestPause();

        /// <summary>
        ///     Whether it is ok to ask the server to pause this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="PauseCommand" /> can fire.</returns>
        protected abstract bool CanRequestPause();

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        [NotNull]
        public virtual RelayCommand StopCommand => _stopCommand
                                                    ?? (_stopCommand = new RelayCommand(
                                                        RequestStop,
                                                        CanRequestStop));

        protected abstract void RequestStop();

        /// <summary>
        ///     Whether it is ok to ask the server to stop this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="StopCommand" /> can fire.</returns>
        protected abstract bool CanRequestStop();
    }
}