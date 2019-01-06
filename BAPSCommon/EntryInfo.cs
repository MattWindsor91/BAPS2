using System;
using System.Collections.Generic;
using System.Text;

namespace BAPSCommon
{
    public abstract class EntryInfo
    {
        public string Description { get; private set; }
        public virtual string Text => "";

        public EntryInfo(string description)
        {
            Description = description;
        }

        public abstract bool IsAudioItem { get; }
        public abstract bool IsTextItem { get; }
        public abstract bool IsFromLibrary { get; }

        public override string ToString() => Description;
    }

    public class TextEntryInfo : EntryInfo
    {
        public TextEntryInfo(string description, string text) : base(description)
        {
            Text = text;
        }

        public override bool IsAudioItem => false;
        public override bool IsTextItem => true;
        public override bool IsFromLibrary => false;
        public override string Text { get; }
    }

    public class FileEntryInfo : EntryInfo
    {
        public FileEntryInfo(string description) : base(description) { }

        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    public class LibraryEntryInfo : EntryInfo
    {
        public LibraryEntryInfo(string description) : base(description) { }

        public override bool IsAudioItem => true;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => true;
    }

    public class NullEntryInfo : EntryInfo
    {
        public NullEntryInfo() : base("NONE") { }

        public override bool IsAudioItem => false;
        public override bool IsTextItem => false;
        public override bool IsFromLibrary => false;
    }

    /// <summary>
    /// Allows the creation of entries from bapsnet commands.
    /// </summary>
    public static class EntryInfoFactory
    {
        /// <summary>
        /// Creates an <see cref="EntryInfo"/> with the given type and description.
        /// <para>
        /// If the type is 'text', the resulting text item will have no text.
        /// This is acceptable for track lists, but one should use
        /// <see cref="TextEntryInfo(string, string)"/> when text is available.
        /// </para>
        /// </summary>
        /// <param name="type">The bapsnet type of the entry.</param>
        /// <param name="descr">The description of the entry.</param>
        /// <returns>An <see cref="EntryInfo"/> with the correct type
        /// and description.</returns>
        public static EntryInfo Create(Command type, string descr)
        {
            switch (type)
            {
                case Command.FILEITEM:
                    return new FileEntryInfo(descr);
                case Command.TEXTITEM:
                    return new TextEntryInfo(descr, "");
                case Command.LIBRARYITEM:
                    return new LibraryEntryInfo(descr);
                default:
                    return new NullEntryInfo();
            }
        }

    }
}
