using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.Common.Controllers
{
    public interface IPlaylistController
    {
        void SetRepeatMode(RepeatMode newMode);
        void AddFile(DirectoryEntry file);

        /// <summary>
        ///     Asks the BAPS server to delete the track-list item for this
        ///     channel at index <see cref="index" />.
        /// </summary>
        /// <param name="index">The 0-based index of the item to delete.</param>
        void DeleteItemAt(uint index);

        /// <summary>
        ///     Asks the BAPS server to reset this channel, deleting all
        ///     track-list items.
        /// </summary>
        void Reset();

        /// <summary>
        ///     Asks the BAPS server to add a text item. 
        /// </summary>
        /// <param name="text">The body of the text item.</param>
        /// <param name="summary">The optional short title of the text item.</param>
        void AddText(string text, string? summary = null);
    }
}