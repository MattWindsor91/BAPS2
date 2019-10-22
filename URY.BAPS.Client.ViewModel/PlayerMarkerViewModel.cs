using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Client.ViewModel
{
    public class PlayerMarkerViewModel : ViewModelBase, IPlayerMarkerViewModel
    {
        public PlayerMarkerViewModel(IPlaybackController? controller)
        {
            Controller = controller;
            PlaybackEvents = controller?.PlaybackUpdater ?? new EmptyEventFeed();

            _position = MarkerValue(MarkerType.Position)
                .ToProperty(this, x => x.Position, 0u, scheduler: RxApp.MainThreadScheduler);
            _cuePosition = MarkerValue(MarkerType.Cue)
                .ToProperty(this, x => x.CuePosition, 0u, scheduler: RxApp.MainThreadScheduler);
            _introPosition = MarkerValue(MarkerType.Intro)
                .ToProperty(this, x => x.IntroPosition, 0u, scheduler: RxApp.MainThreadScheduler);

            var trackLoad =
                from x in PlaybackEvents.ObserveTrackLoad select x.Track;

            _duration = (from track in trackLoad where track.IsAudioItem select track.Duration).ToProperty(this,
                x => x.Duration, scheduler: RxApp.MainThreadScheduler);
            _remaining = this
                .WhenAnyValue(x => x.Duration, x => x.Position, (duration, position) => duration - position)
                .ToProperty(this, x => x.Remaining, scheduler: RxApp.MainThreadScheduler);

            _positionScale = MarkerScale(x => x.Position)
                .ToProperty(this, x => x.PositionScale, scheduler: RxApp.MainThreadScheduler);
            _cuePositionScale = MarkerScale(x => x.CuePosition)
                .ToProperty(this, x => x.CuePositionScale, scheduler: RxApp.MainThreadScheduler);
            _introPositionScale = MarkerScale(x => x.IntroPosition)
                .ToProperty(this, x => x.IntroPositionScale, scheduler: RxApp.MainThreadScheduler);

            var hasLoadedAudioTrack =
                (from track in trackLoad where !track.IsTextItem select track.IsAudioItem).StartWith(false).ObserveOn(
                    RxApp.MainThreadScheduler);
            SetPosition = ReactiveCommand.Create<uint>(SetPositionImpl, hasLoadedAudioTrack);
            SetCue = ReactiveCommand.Create<uint>(SetCueImpl, hasLoadedAudioTrack);
            SetIntro = ReactiveCommand.Create<uint>(SetIntroImpl, hasLoadedAudioTrack);

            _canSetMarkers = hasLoadedAudioTrack.ToProperty(this, x => x.CanSetMarkers);
        }


        private IPlaybackController? Controller { get; }

        /// <summary>
        ///     An event feed that tracks playback events on this channel.
        /// </summary>
        private IPlaybackEventFeed PlaybackEvents { get; }


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
        ///     An <see cref="IObservable{T}"/> that computes the
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

        #region Marker scales

        #endregion Marker values


        private readonly ObservableAsPropertyHelper<double> _positionScale;
        public double PositionScale => _positionScale.Value;

        private readonly ObservableAsPropertyHelper<double> _cuePositionScale;
        public double CuePositionScale => _cuePositionScale.Value;

        private readonly ObservableAsPropertyHelper<double> _introPositionScale;
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
        private IObservable<double> MarkerScale(Expression<Func<PlayerMarkerViewModel, uint>> markerExpression)
        {
            return this.WhenAnyValue(markerExpression, x => x.Duration,
                (marker, duration) => duration == 0 ? 0 : (double) marker / duration);
        }

        #endregion Marker scales

        #region Commands

        private readonly ObservableAsPropertyHelper<bool> _canSetMarkers;
        public bool CanSetMarkers => _canSetMarkers.Value;

        public ReactiveCommand<uint, Unit> SetPosition { get; }

        public ReactiveCommand<uint, Unit> SetCue { get; }

        public ReactiveCommand<uint, Unit> SetIntro { get; }

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

        #endregion Commands
    }
}