using URY.BAPS.Protocol.V2.Commands;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="PlaybackCommand"/>.
    /// </summary>
    public class PlaybackCommandTests
    {
        /// <summary>
        ///     Tests that a playback command with no non-channel arguments packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_ChannelOnly()
        {
            var expected = PlaybackOp.Play.AsCommandWord().WithChannel(42);

            var unpacked = new PlaybackCommand(PlaybackOp.Play, 42);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}
