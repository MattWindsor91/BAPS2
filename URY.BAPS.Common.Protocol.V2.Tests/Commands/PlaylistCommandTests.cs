using URY.BAPS.Common.Protocol.V2.Commands;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="PlaylistCommand"/>.
    /// </summary>
    public class PlaylistCommandTests
    {
        /// <summary>
        ///     Tests that a playlist command with no non-channel arguments packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_ChannelOnly()
        {
            var expected = PlaylistOp.ResetPlaylist.AsCommandWord().WithChannel(63);

            var unpacked = new PlaylistCommand(PlaylistOp.ResetPlaylist, 63);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}
