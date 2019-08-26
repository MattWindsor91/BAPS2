using System;
using System.Linq;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for low-level BapsNet commands and their extensions.
    /// </summary>
    public class CommandWordTests
    {
        public static TheoryData<ushort[]> MaskCoverageData =>
            // Each of these implicitly contains the group mask.
            new TheoryData<ushort[]>
            {
                // 'normal'
                new[] {CommandMasks.Op, CommandMasks.ModeFlag, CommandMasks.Value},
                // 'config'
                new[]
                {
                    CommandMasks.Op, CommandMasks.ModeFlag, CommandMasks.ConfigIndexedFlag, CommandMasks.ConfigIndex
                },
                // 'channel'
                new[] {CommandMasks.ChannelOp, CommandMasks.ChannelModeFlag, CommandMasks.ChannelId}
            };

        public static TheoryData<CommandGroup> CommandGroupData =>
            new TheoryData<CommandGroup>
            {
                CommandGroup.Playback,
                CommandGroup.Playlist,
                CommandGroup.Config,
                CommandGroup.Database,
                CommandGroup.System
            };

        /// <summary>
        ///     Checks that the given set of masks covers the full range of a
        ///     command word.
        /// </summary>
        /// <param name="masks">A list of masks that, taken together and with the group mask, should cover the whole command word.</param>
        [Theory]
        [MemberData(nameof(MaskCoverageData))]
        public void TestMaskCoverage(ushort[] masks)
        {
            var finalMask = masks.Aggregate(CommandMasks.Group, (current, mask) => (ushort) (current | mask));
            Assert.Equal((ushort) 0b11111111_11111111, finalMask);
        }

        /// <summary>
        ///     Tests that representative commands from each group return the
        ///     correct <see cref="CommandGroup" /> under <see cref="CommandUnpacking.Group" />.
        /// </summary>
        [Theory]
        [MemberData(nameof(CommandGroupData))]
        public void TestCommandGroup(CommandGroup expectedGroup)
        {
            Assert.Equal(expectedGroup, CommandUnpacking.Group(expectedGroup.ToWordBits()));
        }
    }
}