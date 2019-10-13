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
        /// <summary>
        ///     Constructs a new <see cref="MockPlayerViewModel" /> with a particular channel ID.
        /// </summary>
        /// <param name="channelId">The channel ID to use.</param>
        public MockPlayerViewModel(ushort channelId)
        {
            ChannelId = channelId;

            Play = ReactiveCommand.Create(() => { }, Observable.Return(true));
            Pause = ReactiveCommand.Create(() => { }, Observable.Return(false));
            Stop = ReactiveCommand.Create(() => { }, Observable.Return(false));

            SetPosition = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
            SetCue = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
            SetIntro = ReactiveCommand.Create((uint _) => { }, Observable.Return(true));
        }

        /// <summary>
        ///     Constructs a new <see cref="MockPlayerViewModel" /> with a placeholder channel ID.
        /// </summary>
        public MockPlayerViewModel() : this(0)
        {
        }

        public ushort ChannelId { get; }

        protected PlaybackState State { get; set; } = PlaybackState.Playing;

        public uint StartTime { get; set; } = 0;
        public ITrack LoadedTrack { get; set; } = new FileTrack("Xanadu", 300_000);
        public bool HasLoadedAudioTrack => LoadedTrack.IsAudioItem;
        public bool IsPlaying => State == PlaybackState.Playing;
        public bool IsPaused => State == PlaybackState.Paused;
        public bool IsStopped => State == PlaybackState.Stopped;

        public uint Position { get; set; } = 120_000;
        public uint Duration { get; set; } = 240_000;
        public uint Remaining => Duration - Position;
        public uint CuePosition { get; set; } = 64_000;
        public uint IntroPosition { get; set; } = 20_000;

        public ReactiveCommand<Unit, Unit> Play { get; }
        public ReactiveCommand<Unit, Unit> Pause { get; }
        public ReactiveCommand<Unit, Unit> Stop { get; }
        public ReactiveCommand<uint, Unit> SetCue { get; }
        public ReactiveCommand<uint, Unit> SetPosition { get; }
        public ReactiveCommand<uint, Unit> SetIntro { get; }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}