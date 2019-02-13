using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IChannelViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to channel controls when in design-time mode.
    ///     </para>
    /// </summary>
    public sealed class MockChannelViewModel : ChannelViewModelBase
    {
        /// <summary>
        ///     Constructs a mock channel view model.
        /// </summary>
        /// <param name="channelId">The ID of this channel.</param>
        /// <param name="player">A player view model to use for the playback-specific parts of the model.</param>
        public MockChannelViewModel(ushort channelId, [CanBeNull] IPlayerViewModel player) : base(channelId, player)
        {
            Name = "Mock Channel";

            TrackList.Add(new TrackViewModel(new FileTrack("URY Whisper (Dry)", 2_000)));
            TrackList.Add(new TrackViewModel(new LibraryTrack(
                    "Several Species of Small Furry Animals Gathered Together in a Cave and Grooving with a Pict",
                    36_000))
                {IsLoaded = true});
            TrackList.Add(new TrackViewModel(new TextTrack("Don't Panic", "Always remember where your towel is.")));
        }

        /// <summary>
        ///     Constructs a mock channel view model with sensible defaults.
        /// </summary>
        [UsedImplicitly]
        public MockChannelViewModel() : this(0, new MockPlayerViewModel())
        {
        }

        public override string Name { get; set; }

        public override int SelectedIndex { get; set; } = -1;

        public override bool IsPlayOnLoad { get; set; } = true;
        public override bool IsAutoAdvance { get; set; }
        public override RepeatMode RepeatMode { get; set; } = RepeatMode.One;

        protected override bool CanResetPlaylist()
        {
            return true;
        }

        protected override void ResetPlaylist()
        {
        }

        protected override bool CanDeleteItem()
        {
            return true;
        }

        protected override void DeleteItem()
        {
        }

        protected override bool CanSetRepeatMode(RepeatMode newMode)
        {
            return newMode != RepeatMode;
        }
        
        protected override void SetRepeatMode(RepeatMode newMode)
        {
        }

        protected override bool CanToggleConfig(ChannelFlag setting)
        {
            return setting == ChannelFlag.AutoAdvance;
        }

        protected override void SetConfigFlag(ChannelFlag setting, bool newValue)
        {
            // Deliberately ignored
        }
    }
}