using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    public abstract class TracklistItem
    {
        public string Description { get; }
        public virtual string Text => "";

        protected TracklistItem(string description)
        {
            Description = description;
        }

        public abstract bool IsAudioItem { get; }
        public abstract bool IsTextItem { get; }
        public abstract bool IsFromLibrary { get; }

        public override string ToString() => Description;
    }

    public class TextTracklistItem : TracklistItem
    {
        public TextTracklistItem(string description, string text) : base(description)
        {
            Text = text;
        }

        public override bool IsAudioItem => false;
        public override bool IsTextItem => true;
        public override bool IsFromLibrary => false;
        public override string Text { get; }
    }

    public class FileTracklistItem : TracklistItem
    {
        public FileTracklistItem(string description) : base(description) { }

        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    public class LibraryTracklistItem : TracklistItem
    {
        public LibraryTracklistItem(string description) : base(description) { }

        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => true;
    }

    public class NullTracklistItem : TracklistItem
    {
        public NullTracklistItem() : base("NONE") { }

        public override bool IsAudioItem => false;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    /// <summary>
    /// Allows the creation of entries from bapsnet commands.
    /// </summary>
    public static class TracklistItemFactory
    {
        /// <summary>
        /// Creates an <see cref="TracklistItem"/> with the given type and description.
        /// <para>
        /// If the type is 'text', the resulting text item will have no text.
        /// This is acceptable for track lists, but one should use
        /// <see cref="TextTracklistItem(string, string)"/> when text is available.
        /// </para>
        /// </summary>
        /// <param name="type">The bapsnet type of the entry.</param>
        /// <param name="descr">The description of the entry.</param>
        /// <returns>An <see cref="TracklistItem"/> with the correct type
        /// and description.</returns>
        public static TracklistItem Create(Command type, string descr)
        {
            switch (type)
            {
                case Command.FileItem:
                    return new FileTracklistItem(descr);
                case Command.TextItem:
                    return new TextTracklistItem(descr, "");
                case Command.LibraryItem:
                    return new LibraryTracklistItem(descr);
                default:
                    return new NullTracklistItem();
            }
        }

    }
}
