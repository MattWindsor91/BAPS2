using System;
using System.Diagnostics;
using System.Windows;
using BAPSClientCommon.Controllers;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     View model governing the player head of a channel.
    /// </summary>
    public class PlayerViewModel : PlayerViewModelBase, IDisposable
    {
        private readonly ushort _id;

        private uint _cuePosition;
        private uint _introPosition;

        /// <summary>
        ///     The currently loaded track.
        /// </summary>
        [NotNull] private ITrack _loadedTrack = new NullTrack();

        private uint _position;

        private uint _startTime;
        private PlaybackState _state;

        public PlayerViewModel(ushort id, [CanBeNull] IPlaybackController controller)
        {
            _id = id;
            Controller = controller;

            RegisterForServerUpdates();
        }

        public PlayerViewModel() : this(0, null)
        {
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
                Application.Current.Dispatcher.Invoke(PlayCommand.RaiseCanExecuteChanged);
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

        [CanBeNull] private IPlaybackController Controller { get; }

        public void Dispose()
        {
            UnregisterForServerUpdates();
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

        private void RegisterForServerUpdates()
        {
            if (Controller == null) return;
            Controller.PlaybackUpdater.ChannelState += HandlePlayerState;
            Controller.PlaybackUpdater.ChannelMarker += HandleMarker;
            Controller.PlaybackUpdater.TrackLoad += HandleTrackLoad;
        }

        private void UnregisterForServerUpdates()
        {
            if (Controller == null) return;
            Controller.PlaybackUpdater.ChannelState -= HandlePlayerState;
            Controller.PlaybackUpdater.ChannelMarker -= HandleMarker;
            Controller.PlaybackUpdater.TrackLoad -= HandleTrackLoad;
        }

        private void HandlePlayerState(object sender, Updates.PlayerStateEventArgs id)
        {
            if (id.ChannelId != _id) return;
            State = id.State;
        }

        private void HandleMarker(object sender, Updates.MarkerEventArgs args)
        {
            if (args.ChannelId != _id) return;

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
            Debug.Assert(Controller != null, nameof(Controller) + " != null");
            Controller.SetState(PlaybackState.Playing);
        }

        protected override void RequestPause()
        {
            Debug.Assert(Controller != null, nameof(Controller) + " != null");
            Controller.SetState(PlaybackState.Paused);
        }

        protected override void RequestStop()
        {
            Debug.Assert(Controller != null, nameof(Controller) + " != null");
            Controller.SetState(PlaybackState.Stopped);
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

        private void HandleTrackLoad(object sender, Updates.TrackLoadEventArgs args)
        {
            if (args.ChannelId != _id) return;

            var track = args.Track;
            LoadedTrack = track;
            if (!(track.IsAudioItem || track.IsTextItem)) Zero();
        }
    }
}