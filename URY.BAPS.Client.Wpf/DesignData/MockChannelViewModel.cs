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
        ///     Constructs a mock channel view model with specific parameters.
        /// </summary>
        /// <param name="channelId">The ID of this channel.</param>
        /// <param name="player">A player view model to use for the playback-specific parts of the model.</param>
        /// <param name="trackList">A track-list view model to use for the track-list-specific parts of the model</param>
        public MockChannelViewModel(ushort channelId,
            [CanBeNull] IPlayerViewModel player,
            [CanBeNull] ITrackListViewModel trackList) : base(channelId, player, trackList)
        {
            Name = "Mock Channel";
        }

        /// <summary>
        ///     Constructs a mock channel view model with placeholder parameters.
        /// </summary>
        [UsedImplicitly]
        public MockChannelViewModel() : this(0, new MockPlayerViewModel(), new MockTrackListViewModel())
        {
        }

        public override string Name { get; set; }


        public override bool IsPlayOnLoad { get; set; } = true;
        public override bool IsAutoAdvance { get; set; }
        public override RepeatMode RepeatMode { get; set; } = RepeatMode.One;

        protected override bool CanOpenAudioWall()
        {
            return true;
        }

        protected override void OpenAudioWall()
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

        public override void Dispose()
        {
            // Nothing to dispose
        }
    }
}