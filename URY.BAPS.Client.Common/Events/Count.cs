namespace URY.BAPS.Client.Common.Events
{
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

    public struct CountEventArgs
    {
        public CountType Type;
        public uint Count;
        public uint Extra;
    }
}