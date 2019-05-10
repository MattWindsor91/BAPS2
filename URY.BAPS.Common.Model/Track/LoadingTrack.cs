namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     A special track that represents the fact that the server is
    ///     currently loading a track.
    /// </summary>
    public class LoadingTrack : SpecialTrackBase
    {
        public LoadingTrack() : base("Loading")
        {
        }

        public override bool IsLoading => true;
        public override bool IsError => false;
    }
}