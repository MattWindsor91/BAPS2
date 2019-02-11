using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
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
        
        /// <summary>
        ///     The list of handles to observable subscriptions that this view model creates.
        /// </summary>
        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        public PlayerViewModel(ushort id, [CanBeNull] IPlaybackController controller)
        {
            _id = id;
            Controller = controller;

            SubscribeToServerUpdates();
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
            UnsubscribeFromServerUpdates();
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
        
        /// <summary>
        ///     Restricts a channel observable to returning only events for this player's channel.
        /// </summary>
        /// <param name="source">The observable to filter.</param>
        /// <typeparam name="TResult">Type of output from the observable.</typeparam>
        /// <returns>The observable corresponding to filtering <see cref="source"/> to events for this player's channel.</returns>
        [NotNull, Pure]
        private IObservable<TResult> OnThisPlayer<TResult>(IObservable<TResult> source)
            where TResult : ChannelEventArgs
        {
            return from ev in source where ev.ChannelId == _id select ev;
        }
        
        private void SubscribeToServerUpdates()
        {
            if (Controller == null) return;

            var updater = Controller.PlaybackUpdater;
            _subscriptions.Add(OnThisPlayer(updater.ObservePlayerState).Subscribe(HandlePlayerState));
            _subscriptions.Add(OnThisPlayer(updater.ObserveMarker).Subscribe(HandleMarker));
            _subscriptions.Add(OnThisPlayer(updater.ObserveTrackLoad).Subscribe(HandleTrackLoad));
        }

        private void UnsubscribeFromServerUpdates()
        {
            foreach (var subscription in _subscriptions) subscription.Dispose();
        }

        private void HandlePlayerState(Updates.PlayerStateEventArgs id)
        {
            State = id.State;
        }

        private void HandleMarker(Updates.MarkerEventArgs args)
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

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            if (args.ChannelId != _id) return;

            var track = args.Track;
            LoadedTrack = track;
            if (!(track.IsAudioItem || track.IsTextItem)) Zero();
        }
    }
}