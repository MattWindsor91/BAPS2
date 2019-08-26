using System;
using URY.BAPS.Common.Protocol.V2.Commands;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    using static CommandUnpacking;

    /// <summary>
    ///     Tests for the <see cref="CommandUnpacking"/> static class.
    /// </summary>
    public class CommandUnpackingTests
    {
        public static TheoryData<ushort> ChannelRoundTripData =>
            new TheoryData<ushort>
            {
                0, 1, 2, 3
            };

        /// <summary>
        ///     Tests that, if we pack a channel ID with
        ///     <see cref="CommandPacking.Channel" />, then the result of
        ///     <see cref="CommandUnpacking.Channel" /> is equal to that channel.
        /// </summary>
        [Theory]
        [MemberData(nameof(ChannelRoundTripData))]
        public void TestChannelRoundTrip(byte channelId)
        {
            Assert.Equal(channelId, Channel(CommandPacking.Channel(channelId)));
        }

        /// <summary>
        ///     Tests that trying to unpack a playback operation with
        ///     no 
        /// </summary>
        [Fact]
        public void TestPlaybackOp_OutOfRange()
        {
            // Assuming that the playback opcode with all bits set will never be used.
            Assert.Throws<ArgumentOutOfRangeException>(() => PlaybackOp(ushort.MaxValue));
        }
    }
}