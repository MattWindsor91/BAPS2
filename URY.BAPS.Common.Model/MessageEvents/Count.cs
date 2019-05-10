namespace URY.BAPS.Common.Model.MessageEvents
{
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
        LibraryItem,
        Show,
        Listing,
        ConfigOption,
        ConfigChoice,
        User,
        Permission,
        IpRestriction
    }
}