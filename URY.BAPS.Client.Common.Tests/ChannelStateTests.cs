using URY.BAPS.Client.Common.Model;
using Xunit;

namespace URY.BAPS.Client.Common.Tests
{
    /// <summary>
    ///     Tests methods related to <see cref="PlaybackState" />.
    /// </summary>
    public class ChannelStateTests
    {
        public static TheoryData<PlaybackState> ChannelStateData =>
            new TheoryData<PlaybackState> {PlaybackState.Paused, PlaybackState.Playing, PlaybackState.Stopped};

        /// <summary>
        ///     Checks that converting a state to a playback operation, then back, does nothing.
        /// </summary>
        /// <param name="state">The state to test.</param>
        [Theory, MemberData(nameof(ChannelStateData))]
        public void TestChannelStateOpRoundTrip(PlaybackState state)
        {
            Assert.Equal(state, state.AsPlaybackOp().AsPlaybackState());
        }

        /// <summary>
        ///     Checks that converting a state to a command, then back, does nothing.
        /// </summary>
        /// <param name="state">The state to test.</param>
        [Theory, MemberData(nameof(ChannelStateData))]
        public void TestChannelStateCommandRoundTrip(PlaybackState state)
        {
            Assert.Equal(state, state.AsCommandWord().AsPlaybackState());
        }
    }
}