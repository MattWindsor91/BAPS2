namespace URY.BAPS.Common.Protocol.V2.Model
{
    /// <summary>
    ///     Enumeration of BapsNet track types.
    ///     <para>
    ///         The values of each enum constant correspond directly to the
    ///         underlying BapsNet code, and therefore must remain in sync
    ///         with the server protocol.
    ///     </para>
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        ///     Absence of track type, used in erroneous circumstances.
        /// </summary>
        Void = 0,

        /// <summary>
        ///     An audio track taken from a local directory file.
        /// </summary>
        File = 1,

        /// <summary>
        ///     An audio track taken from the central library.
        /// </summary>
        Library = 2,

        /// <summary>
        ///     A text item.
        /// </summary>
        Text = 3
    }

    public static class TrackTypeExtensions
    {
        /// <summary>
        ///     Gets whether this track type denotes an audio file.
        ///     <para>
        ///         This lets command decoders decide whether a duration is needed.
        ///     </para>
        /// </summary>
        /// <param name="tt">The track type to inspect.</param>
        /// <returns>
        ///     True if, and only if, <see cref="tt" /> references a file or
        ///     library item.
        /// </returns>
        public static bool HasAudio(this TrackType tt)
        {
            return tt == TrackType.File || tt == TrackType.Library;
        }

        /// <summary>
        ///     Gets whether this track type denotes a text entry.
        ///     <para>
        ///         This lets command decoders decide whether text is needed.
        ///     </para>
        /// </summary>
        /// <param name="tt">The track type to inspect.</param>
        /// <returns>
        ///     True if, and only if, <see cref="tt" /> references a text item.
        /// </returns>
        public static bool HasText(this TrackType tt)
        {
            return tt == TrackType.Text;
        }
    }
}