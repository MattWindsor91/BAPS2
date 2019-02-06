using BAPSClientCommon.Model;
using BAPSPresenterNG.ViewModel;

namespace BAPSPresenterNG.DesignData
{
    public class MockPlayerViewModel : PlayerViewModelBase
    {
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