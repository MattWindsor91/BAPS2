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

        /// <summary>
        ///     Tests that, if we add a channel to a command with
        ///     <see cref="CommandExtensions.WithChannel" />, then the result of
        ///     <see cref="CommandExtensions.Channel" /> is equal to that channel.
        /// </summary>
        [Theory, MemberData(nameof(ChannelRoundTripData))]
        public void TestChannelRoundTrip(ushort channelId)
        {
            const Command cmd = Command.Playback | Command.Play;
            Assert.Equal(channelId, cmd.WithChannel(channelId).Channel());
        }

        public static TheoryData<Command, CommandGroup> CommandGroupData =>
            new TheoryData<Command, CommandGroup>
            {
                { Command.Playback | Command.Play, CommandGroup.Playback },
                { Command.Playlist | Command.AddItem, CommandGroup.Playlist },
                { Command.Config | Command.GetOption, CommandGroup.Config },
                { Command.Database | Command.GetShows, CommandGroup.Database },
                { Command.System | Command.Quit, CommandGroup.System }
            };

        /// <summary>
        ///     Tests that representative commands from each group return the
        ///     correct <see cref="CommandGroup" /> under <see cref="CommandExtensions.Group" />.
        /// </summary>
        [Theory, MemberData(nameof(CommandGroupData))]
        public void TestCommandGroup(Command command, CommandGroup expectedGroup)
        {
            Assert.Equal(expectedGroup, command.Group());
        }
    }
}