namespace URY.BAPS.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form the BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of playback operations.
    /// </summary>
    public enum PlaybackOp : byte
    {
        Play = 0,
        Stop = 1,
        Pause = 2,
        Position = 3,
        Volume = 4,
        Load = 5,
        CuePosition = 6,
        IntroPosition = 7
    }
}