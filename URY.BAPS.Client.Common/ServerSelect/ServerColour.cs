namespace URY.BAPS.Client.Common.ServerSelect
{
    /// <summary>
    ///     An enumeration of colours that can be assigned to a <see cref="ServerRecord" />.
    ///     <para>
    ///         Why can servers take colours?  Because, in URY at least, most servers relate to a particular studio, and
    ///         each studio has a colour (Studio Red, Studio Blue, etc).  As such, the colour is a useful visual aid
    ///         when selecting the correct server in the BAPS client.
    ///     </para>
    /// </summary>
    public enum ServerColour
    {
        /// <summary>
        ///     This server has no particular colour assigned.
        /// </summary>
        None,

        /// <summary>
        ///     This server is red.
        /// </summary>
        Red,

        /// <summary>
        ///     This server is green.
        /// </summary>
        Green,

        /// <summary>
        ///     This server is yellow.
        /// </summary>
        Yellow,

        /// <summary>
        ///     This server is blue.
        /// </summary>
        Blue
    }
}