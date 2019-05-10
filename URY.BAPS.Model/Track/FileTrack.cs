namespace URY.BAPS.Model.Track
{
    /// <summary>
    ///     A track that represents a non-library audio file.
    /// </summary>
    public class FileTrack : AudioTrackBase
    {
        public FileTrack(string description, uint duration) : base(description, duration)
        {
        }

        public override bool IsFromLibrary => false;
    }
}