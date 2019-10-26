using System;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     ReactiveUI implementation of <see cref="IPlayerTrackViewModel"/>.
    /// </summary>
    public class PlayerTrackViewModel : ViewModelBase, IPlayerTrackViewModel
    {
        public PlayerTrackViewModel(IPlaybackController? controller)
        {
            var playbackEvents = controller?.PlaybackUpdater ?? new EmptyEventFeed();
            _loadedTrack =
                NonTextLoadedTrack(playbackEvents).ToProperty(this,
                    x => x.LoadedTrack, new NullTrack(), scheduler: RxApp.MainThreadScheduler);
        }

        private readonly ObservableAsPropertyHelper<ITrack> _loadedTrack;

        public ITrack LoadedTrack => _loadedTrack.Value;

        /// <summary>
        ///     Creates an observable tracking the most recently loaded
        ///     non-text track.
        ///     <para>
        ///         The most recently loaded text track generally gets handled
        ///         in the <see cref="TextViewModel"/>.
        ///     </para>
        /// </summary>
        /// <param name="playbackEvents">
        ///     The events feed to adapt for the observable.
        /// </param>
        /// <returns>
        ///     An <see cref="IObservable{T}"/> that tracks the most
        ///     recent track sent from <paramref name="playbackEvents"/> that
        ///     is not a text item.
        /// </returns>
        private static IObservable<ITrack> NonTextLoadedTrack(IPlaybackEventFeed playbackEvents) => from x in playbackEvents.ObserveTrackLoad where !x.Track.IsTextItem select x.Track;

        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            _loadedTrack.Dispose();
        }
    }
}