using System;
using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;
using ArgumentNullException = System.ArgumentNullException;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     View model governing the player head of a channel.
    /// </summary>
    public class PlayerViewModel : PlayerViewModelBase
    {
        private uint _cuePosition;
        private uint _introPosition;

        /// <summary>
        ///     The currently loaded track.
        /// </summary>
        [NotNull] private ITrack _loadedTrack = new NullTrack();

        private uint _position;

        private uint _startTime;
        private PlaybackState _state;

        public PlayerViewModel(ushort channelId, IPlaybackController? controller) : base(channelId)
        {
            Controller = controller;

            SubscribeToServerUpdates();
        }

        /// <summary>
        ///     The expected start time of the currently loaded item (if any).
        /// </summary>
        public override uint StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime == value) return;
                _startTime = value;
                RaisePropertyChanged(nameof(StartTime));
            }
        }

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        public override ITrack LoadedTrack
        {
            get => _loadedTrack;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_loadedTrack == value) return;

                _loadedTrack = value;
                RaisePropertyChanged(nameof(LoadedTrack));
                // Transitive dependency on LoadedTrack
                RaisePropertyChanged(nameof(HasLoadedAudioTrack));
                RaisePropertyChanged(nameof(Duration));
                // Transitive dependency on Duration
                RaisePropertyChanged(nameof(Remaining));
                RaisePropertyChanged(nameof(PositionScale));
                RaisePropertyChanged(nameof(CuePositionScale));
                RaisePropertyChanged(nameof(IntroPositionScale));
            }
        }

        protected override PlaybackState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                Application.Current.Dispatcher?.Invoke(PlayCommand.RaiseCanExecuteChanged);
                RaisePropertyChanged(nameof(State));
                // Derived properties
                RaisePropertyChanged(nameof(IsPlaying));
                RaisePropertyChanged(nameof(IsPaused));
                RaisePropertyChanged(nameof(IsStopped));
            }
        }

        /// <summary>
        ///     The position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public override uint Position
        {
            get => _position;
            set
            {
                if (_position == value) return;
                _position = value;
                RaisePropertyChanged(nameof(Position));
                // Transitive dependency on Position
                RaisePropertyChanged(nameof(PositionScale));
                RaisePropertyChanged(nameof(Remaining));
            }
        }

        /// <summary>
        ///     The cue position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public override uint CuePosition
        {
            get => _cuePosition;
            set
            {
                if (_cuePosition == value) return;
                _cuePosition = value;
                RaisePropertyChanged(nameof(CuePosition));
                RaisePropertyChanged(nameof(CuePositionScale));
            }
        }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        public override uint IntroPosition
        {
            get => _introPosition;
            set
            {
                if (_introPosition == value) return;
                _introPosition = value;
                RaisePropertyChanged(nameof(IntroPosition));
                RaisePropertyChanged(nameof(IntroPositionScale));
            }
        }

        private bool HasController => Controller != null;

        private IPlaybackController? Controller { get; }

        protected override void RequestSetCue(uint newCue)
        {
            // NOTE: this, and the similar early-returns in the other markers, serve two purposes.
            // They prevent redundant server traffic, but *also* guard against the UI view
            // responding to *server updates* on markers by sending the server a change request.
            // Doing so not only wastes network resources by telling the BAPS server to move to the
            // position it just told us it moved to, but can also cause audio judder if the position
            // moved during the round-trip.
            //
            // At time of writing, the WPF player view *does* do this.
            if (newCue == CuePosition) return;
            Controller?.SetMarker(MarkerType.Cue, newCue);
        }

        protected override bool CanRequestSetCue(uint newCue)
        {
            return HasLoadedAudioTrack;
        }

        protected override void RequestSetIntro(uint newIntro)
        {
            if (newIntro == IntroPosition) return;
            Controller?.SetMarker(MarkerType.Intro, newIntro);
        }

        protected override bool CanRequestSetIntro(uint newIntro)
        {
            return HasLoadedAudioTrack;
        }

        protected override void RequestSetPosition(uint newPosition)
        {
            if (newPosition == Position) return;
            Controller?.SetMarker(MarkerType.Position, newPosition);
        }

        protected override bool CanRequestSetPosition(uint newPosition)
        {
            return HasLoadedAudioTrack;
        }

        [Pure]
        protected override bool CanRequestPlay()
        {
            return HasController && !IsPlaying;
        }

        [Pure]
        protected override bool CanRequestPause()
        {
            return HasController;
        }

        [Pure]
        protected override bool CanRequestStop()
        {
            return HasController;
        }

        private void SubscribeToServerUpdates()
        {
            if (Controller == null) return;

            var updater = Controller.PlaybackUpdater;
            SubscribeTo(OnThisChannel(updater.ObservePlayerState), HandlePlayerState);
            SubscribeTo(OnThisChannel(updater.ObserveMarker), HandleMarker);
            SubscribeTo(OnThisChannel(updater.ObserveTrackLoad), HandleTrackLoad);
        }

        private void HandlePlayerState(PlaybackStateChangeArgs id)
        {
            State = id.State;
        }

        private void HandleMarker(MarkerChangeArgs args)
        {
            switch (args.Marker)
            {
                case MarkerType.Position:
                    Position = args.NewValue;
                    break;
                case MarkerType.Cue:
                    CuePosition = args.NewValue;
                    break;
                case MarkerType.Intro:
                    IntroPosition = args.NewValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void RequestPlay()
        {
            SetState(PlaybackState.Playing);
        }

        protected override void RequestPause()
        {
            SetState(PlaybackState.Paused);
        }

        protected override void RequestStop()
        {
            SetState(PlaybackState.Stopped);
        }

        private void SetState(PlaybackState newState)
        {
            Debug.Assert(Controller != null, nameof(Controller) + " != null");
            Controller?.SetState(newState);
        }


        /// <summary>
        ///     Sets all of the channel position markers to zero.
        /// </summary>
        private void Zero()
        {
            Position = 0;
            CuePosition = 0;
            IntroPosition = 0;
        }

        private void HandleTrackLoad(TrackLoadArgs args)
        {
            if (args.ChannelId != ChannelId) return;
            var track = args.Track;
            if (track.IsTextItem) return;

            LoadedTrack = track;
            if (!track.IsAudioItem) Zero();
        }
    }
}