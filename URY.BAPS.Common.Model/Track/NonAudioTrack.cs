namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     Abstract base class for tracks that don't contain audio.
    /// </summary>
    public abstract class NonAudioTrack : TrackBase
    {
        protected NonAudioTrack(string description) : base(description)
        {
        }

        public override uint Duration => 0;
        public override bool IsAudioItem => false;
        public override bool IsFromLibrary => false;
    }
}