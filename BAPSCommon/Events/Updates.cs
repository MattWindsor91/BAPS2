namespace BAPSClientCommon.Events
{
    /// <summary>
    ///     Container for event structures and delegates that represent updates
    ///     from the BAPS server.
    /// </summary>
    public static partial class Updates
    {
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

        /// <summary>
        ///     Enumeration of different errors a BAPS server can send.
        /// </summary>
        public enum ErrorType
        {
            Library,
            BapsDb,
            Config
        }

        public enum UpDown : byte
        {
            Down = 0,
            Up = 1
        }

        public struct CountEventArgs
        {
            public CountType Type;
            public uint Count;
            public uint Extra;
        }


        public struct ErrorEventArgs
        {
            public ErrorType Type;
            public byte Code;
            public string Description;
        }
    }
}