using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Client.ViewModel.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IPlayerTransportViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to player controls in design mode.
    ///     </para>
    /// </summary>
    public class MockPlayerTransportViewModel : IPlayerTransportViewModel
    {
        /// <summary>
        ///     Constructs a new <see cref="MockPlayerTransportViewModel" />.
        /// </summary>
        public MockPlayerTransportViewModel()
        {
            Play = ReactiveCommand.Create(() => { }, Observable.Return(true));
            Pause = ReactiveCommand.Create(() => { }, Observable.Return(false));
            Stop = ReactiveCommand.Create(() => { }, Observable.Return(false));
        }

        protected PlaybackState State { get; set; } = PlaybackState.Playing;

        public bool IsPlaying => State == PlaybackState.Playing;
        public bool IsPaused => State == PlaybackState.Paused;
        public bool IsStopped => State == PlaybackState.Stopped;

        public ReactiveCommand<Unit, Unit> Play { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }

        public void Dispose()
        {
            Play.Dispose();
            Pause.Dispose();
            Stop.Dispose();
        }
    }
}