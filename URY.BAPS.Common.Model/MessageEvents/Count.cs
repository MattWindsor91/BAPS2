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
        public CountArgs(CountType type, uint count, uint extra)
        {
            Type = type;
            Count = count;
            Extra = extra;
        }

        public CountType Type { get; }
        public uint Count { get; }
        public uint Extra { get; }
    }

    /// <summary>
    ///     Enumeration of different counts a BAPS server can send.
    /// </summary>
    public enum CountType
    {
        /// <summary>
        ///     The message is counting playlist items.
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
        ConfigChoice,
        User,
        Permission,
        IpRestriction
    }
}