namespace URY.BAPS.Common.Model.Track
{
    /// <summary>
    ///     Abstract implementation of <see cref="ITrack"/> that
    ///     delegates all properties to another underlying instance of
    ///     <see cref="ITrack"/>.
    /// </summary>
    public abstract class WrappedTrackBase : ITrack
    {
        /// <summary>
        ///     Constructs a <see cref="WrappedTrackBase"/> wrapping
        ///     the given track.
        /// </summary>
        /// <param name="underlyingTrack">The track to wrap.</param>
        protected WrappedTrackBase(ITrack underlyingTrack)
        {
            UnderlyingTrack = underlyingTrack;
        }

        /// <summary>
        ///     The track being wrapped in this <see cref="WrappedTrackBase" />.
        /// </summary>
        protected ITrack UnderlyingTrack { get; }

        public virtual string Description => UnderlyingTrack.Description;

        public virtual string Text => UnderlyingTrack.Text;

        public virtual bool IsAudioItem => UnderlyingTrack.IsAudioItem;

        public virtual bool IsError => UnderlyingTrack.IsError;

        public virtual bool IsLoading => UnderlyingTrack.IsLoading;

        public virtual bool IsTextItem => UnderlyingTrack.IsTextItem;

        public virtual bool IsFromLibrary => UnderlyingTrack.IsFromLibrary;

        public virtual uint Duration => UnderlyingTrack.Duration;
       
    }
}