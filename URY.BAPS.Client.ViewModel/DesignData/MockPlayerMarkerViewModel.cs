using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace URY.BAPS.Client.ViewModel.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IPlayerViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to player controls in design mode.
    ///     </para>
    /// </summary>
    public class MockPlayerMarkerViewModel : IPlayerMarkerViewModel
    {
        public uint Position { get; } = 120_000;
        public uint CuePosition { get; set; } = 64_000;
        public uint IntroPosition { get; set; } = 20_000;
        public uint Duration { get; set; } = 240_000;
        public uint Remaining => Duration - Position;
        public double PositionScale => (double)Position / Duration;
        public double CuePositionScale => (double) CuePosition / Duration;
        public double IntroPositionScale => (double) IntroPosition / Duration;
        public ReactiveCommand<uint, Unit> SetCue { get; } = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
        public ReactiveCommand<uint, Unit> SetPosition { get; } = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
        public ReactiveCommand<uint, Unit> SetIntro { get; } = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
        public bool CanSetMarkers => true;

        public void Dispose()
        {
            SetCue?.Dispose();
            SetPosition?.Dispose();
            SetIntro?.Dispose();
        }
    }


}