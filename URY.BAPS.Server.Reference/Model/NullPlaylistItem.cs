using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Server.Model
{
    /// <summary>
    ///     Null-object implementation of <see cref="IPlaylistItem" />.
    /// </summary>
    public class NullPlaylistItem : NullTrack, IPlaylistItem
    {
        public bool IsSafeToRemove => true;

        public bool HasIndex(uint index)
        {
            return false;
        }

        public void MarkLoaded()
        {
            // Intentionally left blank
        }

        public void MarkUnloaded()
        {
            // Intentionally left blank
        }
    }
}