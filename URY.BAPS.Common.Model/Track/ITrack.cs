namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     Interface for tracks.
    /// </summary>
    public interface ITrack
    {
        string Description { get; }
        string Text { get; }
        bool IsAudioItem { get; }
        bool IsError { get; }
        bool IsLoading { get; }
        bool IsTextItem { get; }
        bool IsFromLibrary { get; }
        uint Duration { get; }
    }
}