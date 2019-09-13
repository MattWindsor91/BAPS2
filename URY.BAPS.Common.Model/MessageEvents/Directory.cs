namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Base class for all directory update event payloads.
    /// </summary>
    public abstract class DirectoryArgsBase : MessageArgsBase
    {
        protected DirectoryArgsBase(ushort directoryId)
        {
            DirectoryId = directoryId;
        }

        /// <summary>
        ///     The ID of the directory this update concerns.
        /// </summary>
        public ushort DirectoryId { get; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload for when the server clears out and renames a directory,
    ///     or creates one if it doesn't exist.
    /// </summary>
    public class DirectoryPrepareArgs : DirectoryArgsBase
    {
        public DirectoryPrepareArgs(ushort directoryId, string name)
            : base(directoryId)
        {
            Name = name;
        }

        /// <summary>
        ///     The new name of the directory.
        /// </summary>
        public string Name { get; }

        public override string ToString()
        {
            return $"DirectoryPrepare: directory {DirectoryId} is '{Name}'";
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Event payload for when the server adds a file to a directory.
    /// </summary>
    public class DirectoryFileAddArgs : DirectoryArgsBase
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

        public override string ToString()
        {
            return $"DirectoryFileAdd: directory {DirectoryId} index {Index} is '{Description}'";
        }
    }
}