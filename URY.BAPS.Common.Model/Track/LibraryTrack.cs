namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     A track that represents music from the record library.
    /// </summary>
    public class LibraryTrack : AudioTrackBase
    {
        public LibraryTrack(string description, uint duration) : base(description, duration)
        {
        }

        public override bool IsFromLibrary => true;
    }
}