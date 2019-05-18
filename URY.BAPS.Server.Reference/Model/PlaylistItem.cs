using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Server.Reference.Model
{
    /// <summary>
    ///     An item in a BAPS server playlist, consisting of an
    ///     <see cref="ITrack"/> and various server-specific pieces
    ///     of information.
    /// </summary>
    public class PlaylistItem : WrappedTrackBase, IPlaylistItem
    {

        private bool _isLoaded;

        public PlaylistItem(ITrack underlyingTrack, uint index)
        : base(underlyingTrack)
        {
            Index = index;
        }

        /// <summary>
        ///     The index of this track in its playlist.
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        ///     Gets whether this playlist item is safe to remove.
        /// </summary>
        public bool IsSafeToRemove => !(IsAudioItem && _isLoaded);

        public bool HasIndex(uint index)
        {
            return Index == index;
        }

        public void MarkLoaded()
        {
            _isLoaded = true;
        }

        public void MarkUnloaded()
        {
            _isLoaded = false;
        }
    }
}