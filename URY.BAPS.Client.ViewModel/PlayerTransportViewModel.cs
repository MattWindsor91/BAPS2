using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     ReactiveUI implementation of <see cref="IPlayerTransportViewModel"/>.
    /// </summary>
    public class PlayerTransportViewModel : ViewModelBase, IPlayerTransportViewModel
    {
        public PlayerTransportViewModel(IPlaybackController? controller)
        {
            Controller = controller;
            PlaybackEvents = controller?.PlaybackUpdater ?? new EmptyEventFeed();

            // Note: the order of this is somewhat sensitive.
            // Observables that reference properties must be instantiated AFTER
            // the observables that back those properties.

            // Things that derive from PlaybackEvents need specifically telling
            // to run on the UI thread.
            _state = (from x in PlaybackEvents.ObservePlayerState select x.State).ToProperty(this, x => x.State, PlaybackState.Stopped, scheduler: RxApp.MainThreadScheduler);
            _isPlaying = PlaybackStateEquals(PlaybackState.Playing).ToProperty(this, x => x.IsPlaying);
            _isPaused = PlaybackStateEquals(PlaybackState.Paused).ToProperty(this, x => x.IsPaused);
            _isStopped = PlaybackStateEquals(PlaybackState.Stopped).ToProperty(this, x => x.IsStopped);

            Play = ReactiveCommand.Create(PlayImpl, CanPlay);
            Pause = ReactiveCommand.Create(PauseImpl, CanPause);
            Stop = ReactiveCommand.Create(StopImpl, CanStop);

        }

        private bool HasController => Controller != null;

        private IPlaybackController? Controller { get; }

        /// <summary>
        ///     An event feed that tracks playback events on this channel.
        /// </summary>
        private IPlaybackEventFeed PlaybackEvents { get; }

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


        #region Commands

        public ReactiveCommand<Unit, Unit> Play { get; }

        public ReactiveCommand<Unit, Unit> Pause { get; }

        public ReactiveCommand<Unit, Unit> Stop { get; }

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

        public override void Dispose()
        {
            _isPlaying.Dispose();
            _isPaused.Dispose();
            _isStopped.Dispose();
            _state.Dispose();
            Play.Dispose();
            Pause.Dispose();
            Stop.Dispose();
            base.Dispose();
        }
    }
}