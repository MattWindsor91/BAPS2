namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     Abstract base class for most track implementations.
    /// </summary>
    public abstract class TrackBase : ITrack
    {
        protected TrackBase(string description)
        {
            Description = description;
        }

        public string Description { get; }
        public abstract string Text { get; }

        public abstract bool IsAudioItem { get; }
        public abstract bool IsError { get; }
        public abstract bool IsLoading { get; }
        public abstract bool IsTextItem { get; }
        public abstract bool IsFromLibrary { get; }
        public abstract uint Duration { get; }

        public override string ToString()
        {
            return Description;
        }
    }
}