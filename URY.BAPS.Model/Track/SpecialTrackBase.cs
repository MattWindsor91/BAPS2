namespace URY.BAPS.Model.Track
{
    /// <summary>
    ///     Abstract base class for tracks that represent a special server
    ///     state.
    /// </summary>
    public abstract class SpecialTrackBase : NonAudioTrack
    {
        protected SpecialTrackBase(string description) : base(description)
        {
        }

        public override bool IsTextItem => false;
        public override string Text => "";
    }
}