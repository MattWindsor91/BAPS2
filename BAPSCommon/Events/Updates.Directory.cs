namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        public delegate void DirectoryFileAddHandler(object sender, DirectoryFileAddArgs e);

        public delegate void DirectoryPrepareHandler(object sender, DirectoryPrepareArgs e);

        /// <inheritdoc />
        /// <summary>
        ///     Event payload for when the server adds a file to a directory.
        /// </summary>
        public class DirectoryFileAddArgs : DirectoryArgs
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

        /// <inheritdoc />
        /// <summary>
        ///     Event payload for when the server clears out and renames a directory,
        ///     or creates one if it doesn't exist.
        /// </summary>
        public class DirectoryPrepareArgs : DirectoryArgs
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
        }
    }
}