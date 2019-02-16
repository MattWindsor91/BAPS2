using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IPlayerViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to player controls in design mode.
    ///     </para>
    /// </summary>
    public class MockPlayerViewModel : PlayerViewModelBase
    {
        /// <summary>
        ///     Constructs a new <see cref="MockPlayerViewModel" /> with a particular channel ID.
        /// </summary>
        /// <param name="channelId">The channel ID to use.</param>
        public MockPlayerViewModel(ushort channelId) : base(channelId)
        {
        }

        /// <summary>
        ///     Constructs a new <see cref="MockPlayerViewModel" /> with a placeholder channel ID.
        /// </summary>
        public MockPlayerViewModel() : this(0)
        {
        }

        protected override PlaybackState State { get; set; } = PlaybackState.Playing;

        public override uint StartTime { get; set; } = 0;
        public override ITrack LoadedTrack { get; set; } = new FileTrack("Xanadu", 300_000);

        public override uint Position
        {
            get => 120_000;
            set { }
        }

        public override uint CuePosition { get; set; }
        public override uint IntroPosition { get; set; }

        protected override void RequestPlay()
        {
        }

        protected override bool CanRequestPlay()
        {
            return false;
        }

        protected override void RequestPause()
        {
        }

        protected override bool CanRequestPause()
        {
            return true;
        }

        protected override void RequestStop()
        {
        }

        protected override bool CanRequestStop()
        {
            return true;
        }
    }
}