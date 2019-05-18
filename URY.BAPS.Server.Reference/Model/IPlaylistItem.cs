using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Server.Reference.Model
{
    /// <summary>
    ///     Interface for playlist items.
    /// </summary>
    public interface IPlaylistItem : ITrack
    {
        /// <summary>
        ///     Gets whether this playlist item is safe to remove.
        /// </summary>
        bool IsSafeToRemove { get; }

        /// <summary>
        ///     Gets whether this playlist item is currently
        ///     attached to a playlist at the given index.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>
        ///     True if, and only if, this playlist item is on a playlist
        ///     and its index is <paramref name="index"/>.
        /// </returns>
        bool HasIndex(uint index);
        
        void MarkLoaded();
        void MarkUnloaded();
    }
}