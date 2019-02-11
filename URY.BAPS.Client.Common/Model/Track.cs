namespace URY.BAPS.Client.Common.Model
{
    /// <summary>
    ///     Enumeration of BapsNet track types.
    ///     <para>
    ///         The values of each enum constant correspond directly to the
    ///         underlying BapsNet code, and therefore must remain in sync
    ///         with the server protocol.
    ///     </para>
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        ///     Absence of track type, used in erroneous circumstances.
        /// </summary>
        Void = 0,

        /// <summary>
        ///     An audio track taken from a local directory file.
        /// </summary>
        File = 1,

        /// <summary>
        ///     An audio track taken from the central library.
        /// </summary>
        Library = 2,

        /// <summary>
        ///     A text item.
        /// </summary>
        Text = 3
    }

    public static class TrackTypeExtensions
    {
        /// <summary>
        ///     Gets whether this track type denotes an audio file.
        ///     <para>
        ///         This lets command decoders decide whether a duration is needed.
        ///     </para>
        /// </summary>
        /// <param name="tt">The track type to inspect.</param>
        /// <returns>
        ///     True if, and only if, <see cref="tt" /> references a file or
        ///     library item.
        /// </returns>
        public static bool HasAudio(this TrackType tt)
        {
            return tt == TrackType.File || tt == TrackType.Library;
        }

        /// <summary>
        ///     Gets whether this track type denotes a text entry.
        ///     <para>
        ///         This lets command decoders decide whether text is needed.
        ///     </para>
        /// </summary>
        /// <param name="tt">The track type to inspect.</param>
        /// <returns>
        ///     True if, and only if, <see cref="tt" /> references a text item.
        /// </returns>
        public static bool HasText(this TrackType tt)
        {
            return tt == TrackType.Text;
        }
    }

    /// <summary>
    ///     Interface for tracks.
    /// </summary>
    public interface ITrack
    {
        string Description { get; }
        string Text { get; }
        bool IsAudioItem { get; }
        bool IsTextItem { get; }
        bool IsFromLibrary { get; }
        uint Duration { get; }
    }

    /// <summary>
    ///     Abstract base class for most track implementations.
    /// </summary>
    public abstract class Track : ITrack
    {
        protected Track(string description)
        {
            Description = description;
        }

        public string Description { get; }
        public virtual string Text => "";

        public abstract bool IsAudioItem { get; }
        public abstract bool IsTextItem { get; }
        public abstract bool IsFromLibrary { get; }
        public abstract uint Duration { get; }

        public override string ToString()
        {
            return Description;
        }
    }

    public class TextTrack : Track
    {
        public TextTrack(string description, string text) : base(description)
        {
            Text = text;
        }

        public override bool IsAudioItem => false;
        public override bool IsTextItem => true;
        public override bool IsFromLibrary => false;
        public override uint Duration => 0;
        public override string Text { get; }
    }

    public class FileTrack : Track
    {
        public FileTrack(string description, uint duration) : base(description)
        {
            Duration = duration;
        }

        public override uint Duration { get; }
        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    public class LibraryTrack : Track
    {
        public LibraryTrack(string description, uint duration) : base(description)
        {
            Duration = duration;
        }

        public override uint Duration { get; }
        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => true;
    }

    public class NullTrack : Track
    {
        public NullTrack(string description = "NONE") : base(description)
        {
        }

        public override uint Duration => 0;
        public override bool IsAudioItem => false;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    /// <summary>
    ///     Allows the creation of entries from BapsNet commands.
    /// </summary>
    public static class TrackFactory
    {
        /// <summary>
        ///     Creates a <see cref="Track" /> with the given type and description.
        /// </summary>
        /// <param name="type">The BapsNet type of the entry.</param>
        /// <param name="description">The description of the entry.</param>
        /// <param name="duration">If given, the duration to use for any constructed audio items.</param>
        /// <param name="text">If given, the text to use for any constructed text items.</param>
        /// <returns>
        ///     A <see cref="Track" /> with the correct type
        ///     and description.
        /// </returns>
        public static Track Create(TrackType type, string description, uint duration = 0, string text = "")
        {
            switch (type)
            {
                case TrackType.File:
                    return new FileTrack(description, duration);
                case TrackType.Text:
                    return new TextTrack(description, text);
                case TrackType.Library:
                    return new LibraryTrack(description, duration);
                default:
                    return new NullTrack(description);
            }
        }
    }
}