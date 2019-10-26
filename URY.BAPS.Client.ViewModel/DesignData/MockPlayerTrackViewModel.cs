using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel.DesignData
{
    public class MockPlayerTrackViewModel : IPlayerTrackViewModel
    {
        public ITrack LoadedTrack { get; set; } = new FileTrack("Xanadu", 300_000);
    }
}
