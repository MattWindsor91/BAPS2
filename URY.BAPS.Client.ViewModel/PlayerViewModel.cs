using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
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
    public class PlayerViewModel : ViewModelBase, IPlayerViewModel
    {
        public PlayerViewModel(IPlayerTransportViewModel transport, IPlayerMarkerViewModel markers, IPlaybackController? controller)
        {
            Markers = markers;
            Transport = transport;
            PlaybackEvents = controller?.PlaybackUpdater ?? new EmptyEventFeed();

            // Note: the order of this is somewhat sensitive.
            // Observables that reference properties must be instantiated AFTER
            // the observables that back those properties.

            // Things that derive from PlaybackEvents need specifically telling
            // to run on the UI thread.

            _loadedTrack =
                NonTextLoadedTrack.ToProperty(this,
                    x => x.LoadedTrack, new NullTrack(), scheduler:RxApp.MainThreadScheduler);
        }

        public IPlayerMarkerViewModel Markers { get; }

        public IPlayerTransportViewModel Transport { get; }

        /// <summary>
        ///     An event feed that tracks playback events on this channel.
        /// </summary>
        private IPlaybackEventFeed PlaybackEvents { get; }

        #region Loaded track

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
        /// <returns>
        ///     An <see cref="IObservable{ITrack}"/> that tracks the most
        ///     recent track sent from <see cref="PlaybackEvents"/> that
        ///     is not a text item.
        /// </returns>
        private IObservable<ITrack> NonTextLoadedTrack => from x in PlaybackEvents.ObserveTrackLoad where !x.Track.IsTextItem select x.Track;

        #endregion Loaded track

        public override void Dispose()
        {
            Markers.Dispose();
            Transport.Dispose();
            base.Dispose();
        }
    }
}