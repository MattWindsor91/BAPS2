namespace URY.BAPS.Client.Common.Events
{
    /// <inheritdoc />
    /// <summary>
    ///     Event payload for when the server clears out and renames a directory,
    ///     or creates one if it doesn't exist.
    /// </summary>
    public class DirectoryPrepareEventArgs : DirectoryEventArgsBase
    {
        public DirectoryPrepareEventArgs(ushort directoryId, string name)
            : base(directoryId)
        {
            Name = name;
        }

        /// <summary>
        ///     The new name of the directory.
        /// </summary>
        public string Name { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload for when the server adds a file to a directory.
    /// </summary>
    public class DirectoryFileAddArgs : DirectoryEventArgsBase
    {
        public DirectoryFileAddArgs(ushort directoryId, uint index, string description)
            : base(directoryId)
        {
            Index = index;
            Description = description;
        }

        /// <summary>
        ///     The target position of the file in the directory.
        /// </summary>
        public uint Index { get; }

        /// <summary>
        ///     The description of the file.
        /// </summary>
        public string Description { get; }
    }

    /// <summary>
    ///     Base class for all directory update event payloads.
    /// </summary>
    public abstract class DirectoryEventArgsBase
    {
        protected DirectoryEventArgsBase(ushort directoryId)
        {
            DirectoryId = directoryId;
        }

        /// <summary>
        ///     The ID of the directory this update concerns.
        /// </summary>
        public ushort DirectoryId { get; }
    }
}