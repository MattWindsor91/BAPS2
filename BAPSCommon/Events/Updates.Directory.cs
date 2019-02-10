﻿namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <inheritdoc />
        /// <summary>
        ///     Event payload for when the server adds a file to a directory.
        /// </summary>
        public class DirectoryFileAddEventArgs : DirectoryEventArgs
        {
            public DirectoryFileAddEventArgs(ushort directoryId, uint index, string description)
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
        public class DirectoryPrepareEventArgs : DirectoryEventArgs
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
    }
}