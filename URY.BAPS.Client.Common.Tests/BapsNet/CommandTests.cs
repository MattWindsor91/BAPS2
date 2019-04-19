using System.Linq;
using URY.BAPS.Client.Common.BapsNet;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.BapsNet
{
    /// <summary>
    ///     Tests for low-level BapsNet commands and their extensions.
    /// </summary>
    public class CommandTests
    {
        public static TheoryData<ushort> ChannelRoundTripData =>
            new TheoryData<ushort>
            {
                0, 1, 2, 3
            };

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

        public static TheoryData<CommandWord, CommandGroup> CommandGroupData =>
            new TheoryData<CommandWord, CommandGroup>
            {
                {CommandWord.Playback | CommandWord.Play, CommandGroup.Playback},
                {CommandWord.Playlist | CommandWord.AddItem, CommandGroup.Playlist},
                {CommandWord.Config | CommandWord.GetOption, CommandGroup.Config},
                {CommandWord.Database | CommandWord.GetShows, CommandGroup.Database},
                {CommandWord.System | CommandWord.Quit, CommandGroup.System}
            };

        /// <summary>
        ///     Tests that, if we add a channel to a command with
        ///     <see cref="CommandExtensions.WithChannel" />, then the result of
        ///     <see cref="CommandExtensions.Channel" /> is equal to that channel.
        /// </summary>
        [Theory]
        [MemberData(nameof(ChannelRoundTripData))]
        public void TestChannelRoundTrip(byte channelId)
        {
            const CommandWord cmd = CommandWord.Playback | CommandWord.Play;
            Assert.Equal(channelId, cmd.WithChannel(channelId).Channel());
        }

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
        ///     correct <see cref="CommandGroup" /> under <see cref="CommandExtensions.Group" />.
        /// </summary>
        [Theory]
        [MemberData(nameof(CommandGroupData))]
        public void TestCommandGroup(CommandWord commandWord, CommandGroup expectedGroup)
        {
            Assert.Equal(expectedGroup, commandWord.Group());
        }
    }
}