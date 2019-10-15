using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IPlayerViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to player controls in design mode.
    ///     </para>
    /// </summary>
    public class MockPlayerViewModel : IPlayerViewModel
    {
        public IPlayerTransportViewModel Transport { get; } = new MockPlayerTransportViewModel();
        public IPlayerMarkerViewModel Markers { get; } = new MockPlayerMarkerViewModel();

        public ITrack LoadedTrack { get; set; } = new FileTrack("Xanadu", 300_000);

        public void Dispose()
        {
            Markers?.Dispose();
            Transport?.Dispose();
        }
    }
}