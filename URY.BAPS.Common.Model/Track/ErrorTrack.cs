namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     A special track that represents the fact that the server has
    ///     failed to load a track.
    /// </summary>
    public class ErrorTrack : SpecialTrackBase
    {
        public ErrorTrack() : base("Load failed")
        {
        }

        public override bool IsLoading => false;
        public override bool IsError => true;
    }
}