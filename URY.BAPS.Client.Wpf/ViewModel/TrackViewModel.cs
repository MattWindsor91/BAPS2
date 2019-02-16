﻿using System;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     A view model that wraps a <see cref="Track" />, adding tracking for
    ///     whether the item is loaded.
    /// </summary>
    public class TrackViewModel : ViewModelBase, ITrack
    {
        [NotNull] private readonly Track _underlyingTrack;

        private bool _isLoaded;

        public TrackViewModel([CanBeNull] Track underlyingTrack)
        {
            _underlyingTrack = underlyingTrack ?? throw new ArgumentNullException(nameof(underlyingTrack));
        }

        /// <summary>
        ///     Whether or not this track is the loaded track.
        /// </summary>
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                if (_isLoaded == value) return;
                _isLoaded = value;
                RaisePropertyChanged(nameof(IsLoaded));
            }
        }


        public string Description => _underlyingTrack.Description;
        public string Text => _underlyingTrack.Text;
        public bool IsAudioItem => _underlyingTrack.IsAudioItem;
        public bool IsTextItem => _underlyingTrack.IsTextItem;
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