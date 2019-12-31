namespace URY.BAPS.Common.Infrastructure
{
    /// <summary>
    ///     Enumerates the protocols available in the BAPS system.
    ///     <para>
    ///         At time of writing, there is only one protocol: the V2 protocol.  This enum, alongside a great deal of
    ///         other machinery in both client and server, exists to make a future transition to the text-based BAPS3
    ///         protocol smoother.
    ///     </para>
    /// </summary>
    public enum BapsProtocol
    {
        /// <summary>
        ///     Used to select the V2 (binary) protocol.
        /// </summary>
        BapsNetV2
    }
}