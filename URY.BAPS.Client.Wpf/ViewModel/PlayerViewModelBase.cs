namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Abstract base class providing the parts of <see cref="IPlayerViewModel" />
    ///     that are largely the same across implementations.
    /// </summary>
    public abstract class PlayerViewModelBase : ChannelComponentViewModelBase, IPlayerViewModel
    {
        [CanBeNull] private RelayCommand _pauseCommand;
        [CanBeNull] private RelayCommand _playCommand;
        [CanBeNull] private RelayCommand<uint> _setCueCommand;
        [CanBeNull] private RelayCommand<uint> _setIntroCommand;
        [CanBeNull] private RelayCommand<uint> _setPositionCommand;
        [CanBeNull] private RelayCommand _stopCommand;

        protected PlayerViewModelBase(ushort channelId) : base(channelId)
        {
        }

        protected abstract PlaybackState State { get; set; }

        public double PositionScale => (double) Position / Duration;

        public double CuePositionScale => (double) CuePosition / Duration;

        public double IntroPositionScale => (double) IntroPosition / Duration;

        [NotNull]
        public virtual RelayCommand<uint> SetCueCommand => _setCueCommand
                                                           ?? (_setCueCommand = new RelayCommand<uint>(
                                                               RequestSetCue,
                                                               CanRequestSetCue));

        [NotNull]
        public virtual RelayCommand<uint> SetIntroCommand => _setIntroCommand
                                                             ?? (_setIntroCommand = new RelayCommand<uint>(
                                                                 RequestSetIntro,
                                                                 CanRequestSetIntro));

        public abstract uint StartTime { get; set; }
        public abstract ITrack LoadedTrack { get; set; }

        public bool HasLoadedAudioTrack => LoadedTrack.IsAudioItem;

        public bool IsPlaying => State == PlaybackState.Playing;

        public bool IsPaused => State == PlaybackState.Paused;

        public bool IsStopped => State == PlaybackState.Stopped;

        public abstract uint Position { get; set; }

        public uint Duration => LoadedTrack?.Duration ?? 0;

        public uint Remaining => Duration - Position;

        public abstract uint CuePosition { get; set; }

        public abstract uint IntroPosition { get; set; }

        [NotNull]
        public virtual RelayCommand PlayCommand => _playCommand
                                                   ?? (_playCommand = new RelayCommand(
                                                       RequestPlay,
                                                       CanRequestPlay));

        [NotNull]
        public virtual RelayCommand PauseCommand => _pauseCommand
                                                    ?? (_pauseCommand = new RelayCommand(
                                                        RequestPause,
                                                        CanRequestPause));

        [NotNull]
        public virtual RelayCommand StopCommand => _stopCommand
                                                   ?? (_stopCommand = new RelayCommand(
                                                       RequestStop,
                                                       CanRequestStop));

        [NotNull]
        public virtual RelayCommand<uint> SetPositionCommand => _setPositionCommand
                                                                ?? (_setPositionCommand = new RelayCommand<uint>(
                                                                    RequestSetPosition,
                                                                    CanRequestSetPosition));

        public abstract void Dispose();

        protected abstract void RequestSetCue(uint newCue);
        protected abstract bool CanRequestSetCue(uint newCue);

        protected abstract void RequestSetIntro(uint newIntro);
        protected abstract bool CanRequestSetIntro(uint newIntro);

        protected abstract void RequestSetPosition(uint newPosition);
        protected abstract bool CanRequestSetPosition(uint newPosition);


        protected abstract void RequestPlay();

        /// <summary>
        ///     Whether it is ok to ask the server to start playing on this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="PlayCommand" /> can fire.</returns>
        protected abstract bool CanRequestPlay();

        protected abstract void RequestPause();

        /// <summary>
        ///     Whether it is ok to ask the server to pause this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="PauseCommand" /> can fire.</returns>
        protected abstract bool CanRequestPause();

        protected abstract void RequestStop();

        /// <summary>
        ///     Whether it is ok to ask the server to stop this channel.
        /// </summary>
        /// <returns>True provided that the <see cref="StopCommand" /> can fire.</returns>
        protected abstract bool CanRequestStop();
    }
}