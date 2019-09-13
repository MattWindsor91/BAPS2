namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     A server update message that tells the client that a specific
    ///     number of other messages is about to arrive.
    ///     <para>
    ///         Clients can use this information to reserve capacity in
    ///         data structures, check for missing items, and so on.
    ///     </para>
    /// </summary>
    public class CountArgs : MessageArgsBase
    {
        public CountArgs(CountType type, uint count, uint index)
        {
            Type = type;
            Count = count;
            Index = index;
        }

        /// <summary>
        ///     The type of item being counted.
        /// </summary>
        public CountType Type { get; }

        /// <summary>
        ///     The number of incoming items of the specified type.
        /// </summary>
        public uint Count { get; }

        /// <summary>
        ///     If the particular count type refers to an indexed
        ///     item (such as a config option or a channel listing),
        ///     this field contains the index.
        /// </summary>
        public uint Index { get; }

        public override string ToString()
        {
            return $"Count: {Count} item(s) of type {Type} incoming (index {Index})";
        }
    }

    /// <summary>
    ///     Enumeration of different counts a BAPS server can send.
    /// </summary>
    public enum CountType
    {
        /// <summary>
        ///     The message is counting playlist items;
        ///     the index field contains the channel ID.
        /// </summary>
        PlaylistItem,
        /// <summary>
        ///     The message is counting library items.
        /// </summary>
        LibraryItem,
        /// <summary>
        ///     The message is counting shows.
        /// </summary>
        Show,
        /// <summary>
        ///     The message is counting show listings.
        /// </summary>
        Listing,
        ConfigOption,
        /// <summary>
        ///     The message is counting config choices;
        ///     the index field contains the option ID.
        /// </summary>
        ConfigChoice,
        User,
        Permission,
        IpRestriction
    }
}