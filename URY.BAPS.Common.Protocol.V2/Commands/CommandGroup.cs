namespace URY.BAPS.Common.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of command groups.
    /// </summary>
    public enum CommandGroup : byte
    {
        Playback = 0,
        Playlist = 1,
        Database = 3,
        Config = 5,
        System = 7
    }
}