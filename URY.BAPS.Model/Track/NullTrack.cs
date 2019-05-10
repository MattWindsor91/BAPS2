namespace URY.BAPS.Model.Track
{
    /// <summary>
    ///     A special track that represents the absence of a track.
    /// </summary>
    public class NullTrack : SpecialTrackBase
    {
        public NullTrack(string description = "(none)") : base(description)
        {
        }

        public override bool IsLoading => false;
        public override bool IsError => false;
    }
}