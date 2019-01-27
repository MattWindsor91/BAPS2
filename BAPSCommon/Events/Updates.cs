namespace BAPSClientCommon.Events
{
    /// <summary>
    ///     Container for event structures and delegates that represent updates
    ///     from the BAPS server.
    /// </summary>
    public static partial class Updates
    {
        public delegate void CountEventHandler(object sender, CountEventArgs e);

        /// <summary>
        ///     Delegate for handling server errors.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The argument struct, containing the error code and description.</param>
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

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