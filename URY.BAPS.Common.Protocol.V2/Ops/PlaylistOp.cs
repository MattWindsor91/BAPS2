namespace URY.BAPS.Common.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of playlist operations.
    /// </summary>
    public enum PlaylistOp : byte
    {
        AddItem = 0,
        DeleteItem = 1,
        MoveItemTo = 2,
        Item = 3,
        GetPlaylist = 4,
        ResetPlaylist = 5,
        CopyItem = 6
    }
}