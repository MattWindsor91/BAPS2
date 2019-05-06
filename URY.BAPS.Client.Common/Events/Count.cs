namespace URY.BAPS.Client.Common.Events
{
    public class CountArgs : ArgsBase
    {
        public CountType Type { get; }
        public uint Count { get; }
        public uint Extra { get; }

        public CountArgs(CountType type, uint count, uint extra)
        {
            Type = type;
            Count = count;
            Extra = extra;
        }
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