namespace URY.BAPS.Common.Protocol.V2.Model
{
    /// <summary>
    ///     Track descriptions that, when emitted by the BAPS server,
    ///     indicate a 'special' track.
    /// </summary>
    public static class SpecialTrackDescriptions
    {
        /// <summary>
        ///     String used by the BAPS server to represent a lack of track.
        /// </summary>
        public const string None = "--NONE--";

        /// <summary>
        ///     String used by the BAPS server to indicate that it is loading a
        ///     track.
        /// </summary>
        public const string Loading = "--LOADING--";

        /// <summary>
        ///     String used by the BAPS server to indicate that it has failed to
        ///     load a track.
        /// </summary>
        public const string LoadFailed = "--LOAD FAILED--";
    }
}