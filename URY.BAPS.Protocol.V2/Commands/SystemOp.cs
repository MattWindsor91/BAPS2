namespace URY.BAPS.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of system operations.
    /// </summary>
    public enum SystemOp : byte
    {
        ListFiles = 0,
        Filename = 1,
        SendMessage = 2,
        AutoUpdate = 3,
        End = 4,
        SendLogMessage = 5,
        SetBinaryMode = 6,
        Seed = 7,
        Login = 8,
        LoginResult = 9,
        ServerVersion = 10,
        Feedback = 11,
        ClientChange = 12,
        ScrollText = 13,
        TextSize = 14,
        Quit = 15
    }
}