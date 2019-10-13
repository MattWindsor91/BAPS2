using System;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     A view model that wraps a <see cref="TrackBase" />, adding tracking for
    ///     whether the item is loaded.
    /// </summary>
    public class TrackViewModel : ViewModelBase, ITrack
    {
        [NotNull] private readonly ITrack _underlyingTrack;

        private bool _isLoaded;

        public TrackViewModel(ITrack? underlyingTrack)
        {
            _underlyingTrack = underlyingTrack ?? throw new ArgumentNullException(nameof(underlyingTrack));
        }

        /// <summary>
        ///     Whether or not this track is the loaded track.
        /// </summary>
        public bool IsLoaded
        {
            get => _isLoaded;
            set => this.RaiseAndSetIfChanged(ref _isLoaded, value);
        }

        public string Description => _underlyingTrack.Description;
        public string Text => _underlyingTrack.Text;
        public bool IsAudioItem => _underlyingTrack.IsAudioItem;
        public bool IsTextItem => _underlyingTrack.IsTextItem;
        public bool IsError => _underlyingTrack.IsError;
        public bool IsLoading => _underlyingTrack.IsLoading;
        public bool IsFromLibrary => _underlyingTrack.IsFromLibrary;
        public uint Duration => _underlyingTrack.Duration;

        [Pure]
        public override string ToString()
        {
            return _underlyingTrack.ToString();
        }

        /// <summary>
        ///     Makes a track view model for a null track.
        /// </summary>
        /// <returns>
        ///     A <see cref="TrackViewModel" /> whose underlying track is a
        ///     <see cref="NullTrack" />.
        /// </returns>
        [NotNull]
        public static TrackViewModel MakeNull()
        {
            return new TrackViewModel(new NullTrack());
        }
    }
}