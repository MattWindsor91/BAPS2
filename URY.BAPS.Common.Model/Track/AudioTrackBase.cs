namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     Abstract base class for tracks that contain audio.
    /// </summary>
    public abstract class AudioTrackBase : TrackBase
    {
        protected AudioTrackBase(string description, uint duration) : base(description)
        {
            Duration = duration;
        }

        public override uint Duration { get; }
        public override bool IsAudioItem => true;

        public override bool IsError => false;
        public override bool IsLoading => false;

        public override bool IsTextItem => false;
        public override string Text => "";
    }
}