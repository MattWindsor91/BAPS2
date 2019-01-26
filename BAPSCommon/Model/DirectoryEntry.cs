namespace BAPSClientCommon.Model
{
    /// <summary>
    ///     An entry in a Baps directory.
    ///     <para>
    ///         These differ from <see cref="FileTrack" />s in that they
    ///         contain the ID of the directory from which they originate.
    ///         This makes sending a BapsNet command for moving a directory
    ///         entry to a channel, as well as distinguishing incoming
    ///         entries from tracks, easier.
    ///     </para>
    /// </summary>
    public class DirectoryEntry : FileTrack
    {
        /// <summary>
        ///     Constructs a directory entry.
        /// </summary>
        /// <param name="directoryId">The ID of the directory storing this entry.</param>
        /// <param name="description">The description of the entry (generally its filename).</param>
        public DirectoryEntry(ushort directoryId, string description) : base(description)
        {
            DirectoryId = directoryId;
        }

        /// <summary>
        ///     The ID of the directory storing this entry.
        /// </summary>
        public ushort DirectoryId { get; }
    }
}