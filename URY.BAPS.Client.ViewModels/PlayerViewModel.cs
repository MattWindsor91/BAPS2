using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     View model governing the player head of a channel.
    /// </summary>
    public class PlayerViewModel : ChannelComponentViewModelBase, IPlayerViewModel
    {
        public PlayerViewModel(ushort channelId, IPlaybackController? controller, IScheduler? scheduler = null) : base(channelId)
        {
            scheduler ??= RxApp.MainThreadScheduler;

            Controller = controller;
            PlaybackEvents = controller?.PlaybackUpdater ?? new EmptyEventFeed();

            // Note: the order of this is somewhat sensitive.
            // Observables that reference properties must be instantiated AFTER
            // the observables that back those properties.

            // Things that derive from PlaybackEvents need specifically telling
            // to run on the UI thread.
            _state = (from x in PlaybackEvents.ObservePlayerState select x.State).ToProperty(this, x => x.State, PlaybackState.Stopped, scheduler: scheduler);
            _isPlaying = PlaybackStateEquals(PlaybackState.Playing).ToProperty(this, x => x.IsPlaying);
            _isPaused = PlaybackStateEquals(PlaybackState.Paused).ToProperty(this, x => x.IsPaused);
            _isStopped = PlaybackStateEquals(PlaybackState.Stopped).ToProperty(this, x => x.IsStopped);

            _loadedTrack =
                NonTextLoadedTrack().ToProperty(this,
                    x => x.LoadedTrack, new NullTrack(), scheduler:RxApp.MainThreadScheduler);

            Debug.Assert(LoadedTrack != null);

            var hasLoadedAudioTrack = this.WhenAnyValue(x => x.LoadedTrack, track => track.IsAudioItem);
            _hasLoadedAudioTrack = hasLoadedAudioTrack.ToProperty(this, x => x.HasLoadedAudioTrack, scheduler: RxApp.MainThreadScheduler);

            _position = MarkerValue(MarkerType.Position).ToProperty(this, x => x.Position, 0u, scheduler:RxApp.MainThreadScheduler);
            _cuePosition = MarkerValue(MarkerType.Cue).ToProperty(this, x => x.CuePosition, 0u, scheduler:RxApp.MainThreadScheduler);
            _introPosition = MarkerValue(MarkerType.Intro).ToProperty(this, x => x.IntroPosition, 0u, scheduler:RxApp.MainThreadScheduler);

            _duration = this.WhenAnyValue(x => x.LoadedTrack, (ITrack track) => track.Duration)
                .ToProperty(this, x => x.Duration, scheduler: RxApp.MainThreadScheduler);
            _remaining = this
                .WhenAnyValue(x => x.Duration, x => x.Position, (duration, position) => duration - position)
                .ToProperty(this, x => x.Remaining, scheduler: RxApp.MainThreadScheduler);

            _positionScale = MarkerScale(x => x.Position).ToProperty(this, x => x.PositionScale, scheduler: RxApp.MainThreadScheduler);
            _cuePositionScale = MarkerScale(x => x.CuePosition).ToProperty(this, x => x.CuePositionScale, scheduler: RxApp.MainThreadScheduler);
            _introPositionScale = MarkerScale(x => x.IntroPosition).ToProperty(this, x => x.IntroPositionScale, scheduler: RxApp.MainThreadScheduler);

            Play = ReactiveCommand.Create(PlayImpl, CanPlay);
            Pause = ReactiveCommand.Create(PauseImpl, CanPause);
            Stop = ReactiveCommand.Create(StopImpl, CanStop);

            SetPosition = ReactiveCommand.Create<uint>(SetPositionImpl, hasLoadedAudioTrack);
            SetCue = ReactiveCommand.Create<uint>(SetCueImpl, hasLoadedAudioTrack);
            SetIntro = ReactiveCommand.Create<uint>(SetIntroImpl, hasLoadedAudioTrack);
        }

        private bool HasController => Controller != null;

        private IPlaybackController? Controller { get; }

        /// <summary>
        ///     An event feed that tracks playback events on this channel.
        /// </summary>
        private IPlaybackEventFeed PlaybackEvents { get; }

        #region Scheduling

        private uint _startTime;


        /// <summary>
        ///     The expected start time of the currently loaded item (if any).
        /// </summary>
        public uint StartTime
        {
            get => _startTime;
            set => this.RaiseAndSetIfChanged(ref _startTime, value);
        }

        #endregion Scheduling

        #region Commands

        protected void SetCueImpl(uint newCue)
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

        protected void SetIntroImpl(uint newIntro)
        {
            if (newIntro == IntroPosition) return;
            Controller?.SetMarker(MarkerType.Intro, newIntro);
        }

        protected void SetPositionImpl(uint newPosition)
        {
            if (newPosition == Position) return;
            Controller?.SetMarker(MarkerType.Position, newPosition);
        }

        /// <summary>
        ///     Whether it is ok to ask the server to start playing on this channel.
        /// </summary>
        protected IObservable<bool> CanPlay => this.WhenAnyValue(x => x.HasController, x => x.IsPlaying,
                (hasController, isPlaying) => hasController && !isPlaying);

        /// <summary>
        ///     Whether it is ok to ask the server to pause this channel.
        /// </summary>
        protected IObservable<bool> CanPause => this.WhenAnyValue(x => x.HasController);

        /// <summary>
        ///     Whether it is ok to ask the server to stop this channel.
        /// </summary>
        protected IObservable<bool> CanStop => this.WhenAnyValue(x => x.HasController);

        private void SetState(PlaybackState newState)
        {
            Debug.Assert(Controller != null, nameof(Controller) + " != null");
            Controller?.SetState(newState);
        }

        #endregion Commands

        #region Loaded track

        private readonly ObservableAsPropertyHelper<ITrack> _loadedTrack;

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        public ITrack LoadedTrack => _loadedTrack.Value;

        /// <summary>
        ///     Creates an observable tracking the most recently loaded
        ///     non-text track.
        ///     <para>
        ///         The most recently loaded text track generally gets handled
        ///         in the <see cref="TextViewModel"/>.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     An <see cref="IObservable{ITrack}"/> that tracks the most
        ///     recent track sent from <see cref="PlaybackEvents"/> that
        ///     is not a text item.
        /// </returns>
        private IObservable<ITrack> NonTextLoadedTrack()
        {
            return (from x in PlaybackEvents.ObserveTrackLoad where !x.Track.IsTextItem select x.Track);
        }

        private readonly ObservableAsPropertyHelper<bool> _hasLoadedAudioTrack;
        public bool HasLoadedAudioTrack => _hasLoadedAudioTrack.Value;

        #endregion Loaded track

        #region Player state

        private readonly ObservableAsPropertyHelper<PlaybackState> _state;
        public PlaybackState State => _state.Value;

        private readonly ObservableAsPropertyHelper<bool> _isPlaying;

        public bool IsPlaying => _isPlaying.Value;

        private readonly ObservableAsPropertyHelper<bool> _isPaused;

        public bool IsPaused => _isPaused.Value;

        private readonly ObservableAsPropertyHelper<bool> _isStopped;

        public bool IsStopped => _isStopped.Value;

        /// <summary>
        ///     Constructs an observable that tracks whether the current
        ///     playback state equals an expected state.
        /// </summary>
        /// <param name="expected">The expected marker value.</param>
        /// <returns>
        ///     An <see cref="IObservable{Boolean}"/> that returns <c>true</c> when
        ///     incoming values for <see cref="State"/> equal
        ///     <paramref name="expected"/>, and <c>false</c> if not.
        /// </returns>
        private IObservable<bool> PlaybackStateEquals(PlaybackState expected)
        {
            return this.WhenAnyValue(x => x.State, x => x == expected);
        }

        #endregion Player state


        #region Marker values

        private readonly ObservableAsPropertyHelper<uint> _position;
        public uint Position => _position.Value;

        private readonly ObservableAsPropertyHelper<uint> _cuePosition;
        public uint CuePosition => _cuePosition.Value;

        private readonly ObservableAsPropertyHelper<uint> _introPosition;
        public uint IntroPosition => _introPosition.Value;

        private readonly ObservableAsPropertyHelper<uint> _duration;
        public uint Duration => _duration.Value;

        private readonly ObservableAsPropertyHelper<uint> _remaining;
        public uint Remaining => _remaining.Value;

        /// <summary>
        ///     Constructs an observable that tracks the value of a given
        ///     marker.
        ///     <para>
        ///         The marker value is the most recent of 0, the last
        ///         marker value announced by the event feed, and
        ///         a constant marker of 0 emitted very time a non-text,
        ///         non-audio track is loaded.
        ///     </para>
        /// </summary>
        /// <param name="marker">
        ///     The marker whose value is being computed.
        /// </param>
        /// <returns>
        ///     An <see cref="IObservable{Uint32}"/> that computes the
        ///     value of <paramref name="marker"/>
        /// </returns>
        private IObservable<uint> MarkerValue(MarkerType marker)
        {
            var directChanges =
                from x in PlaybackEvents.ObserveMarker
                where x.Marker == marker
                select x.NewValue;

            var trackLoadZero =
                from x in PlaybackEvents.ObserveTrackLoad
                where !x.Track.IsTextItem && !x.Track.IsAudioItem
                select 0u;

            return directChanges.Merge(trackLoadZero);
        }

        #endregion Marker values

        #region Marker scales

        private readonly ObservableAsPropertyHelper<double> _positionScale;
        private readonly ObservableAsPropertyHelper<double> _cuePositionScale;
        private readonly ObservableAsPropertyHelper<double> _introPositionScale;

        public double PositionScale => _positionScale.Value;
        public double CuePositionScale => _cuePositionScale.Value;

        public double IntroPositionScale => _introPositionScale.Value;

        /// <summary>
        ///     Constructs an observable that computes the scaling factor for a
        ///     given marker.
        /// </summary>
        /// <param name="markerExpression">
        ///     The marker whose scale is being computed, given as a LINQ
        ///     expression of the form <c>x => x.MarkerProperty</c>.
        /// </param>
        /// <returns>
        ///     An <see cref="IObservable{Double}"/> that computes the
        ///     scale of the marker in proportion to the duration.
        /// </returns>
        private IObservable<double> MarkerScale(Expression<Func<PlayerViewModel, uint>> markerExpression)
        {
            return this.WhenAnyValue(markerExpression, x => x.Duration, (marker, duration) => duration == 0 ? 0 : (double)marker / duration);
        }

        #endregion Marker scales

        #region Commands

        public ReactiveCommand<Unit, Unit> Play { get; }

        public ReactiveCommand<Unit, Unit> Pause { get; }

        public ReactiveCommand<Unit, Unit> Stop { get; }

        public ReactiveCommand<uint, Unit> SetPosition { get; }

        public ReactiveCommand<uint, Unit> SetCue { get; }

        public ReactiveCommand<uint, Unit> SetIntro { get; }

        protected void PlayImpl()
        {
            SetState(PlaybackState.Playing);
        }

        protected void PauseImpl()
        {
            SetState(PlaybackState.Paused);
        }

        protected void StopImpl()
        {
            SetState(PlaybackState.Stopped);
        }

        #endregion Commands
    }
}