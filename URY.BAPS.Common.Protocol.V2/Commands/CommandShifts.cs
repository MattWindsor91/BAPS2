namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public static class CommandShifts
    {
        /// <summary>
        ///     Amount to shift group codes.
        /// </summary>
        public const ushort Group = 13;

        /// <summary>
        ///     Amount to shift playback and playlist operation codes.
        /// </summary>
        public const ushort ChannelOp = 7;

        /// <summary>
        ///     Amount to shift database, config, and system operation codes.
        /// </summary>
        public const ushort Op = 8;
    }

    /**
     * Library orderings
    **/
    //OrderByArtist = 0,
    //OrderByTitle = 1,
    //OrderByDateAdded = 2,
    //OrderByDateReleased = 3,

    //OrderAscending = 0,
    //OrderDescending = 1,

    //LibraryMaybeDirty = 1,
    //LibraryDirty = 2,
}