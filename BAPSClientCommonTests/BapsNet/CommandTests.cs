using BAPSClientCommon.BapsNet;
using NUnit.Framework;

namespace BAPSClientCommonTests.BapsNet
{
    /// <summary>
    ///     Tests for low-level BapsNet commands and their extensions.
    /// </summary>
    [TestFixture]
    public class CommandTests
    {
        /// <summary>
        ///     Tests that representative commands from each group return the
        ///     correct <see cref="CommandGroup"/> under <see cref="CommandExtensions.Group"/>.
        /// </summary>
        [Test]
        public void TestCommandGroup()
        {
            const Command playbackCmd = Command.Playback | Command.Play;
            Assert.That(playbackCmd.Group(), Is.EqualTo(CommandGroup.Playback));
            
            const Command playlistCmd = Command.Playlist | Command.AddItem;
            Assert.That(playlistCmd.Group(), Is.EqualTo(CommandGroup.Playlist));
            
            const Command configCmd = Command.Config | Command.GetOption;
            Assert.That(configCmd.Group(), Is.EqualTo(CommandGroup.Config));

            const Command databaseCmd = Command.Database | Command.GetShows;
            Assert.That(databaseCmd.Group(), Is.EqualTo(CommandGroup.Database));
            
            const Command systemCmd = Command.System | Command.Quit;
            Assert.That(systemCmd.Group(), Is.EqualTo(CommandGroup.System));
        }

        /// <summary>
        ///     Tests that, if we add a channel to a command with
        ///     <see cref="CommandExtensions.WithChannel"/>, then the result of
        ///     <see cref="CommandExtensions.Channel"/> is equal to that channel.
        /// </summary>
        [Test]
        public void TestChannelRoundTrip([Values((ushort)0, (ushort)1, (ushort)2, (ushort)3)] ushort channelId)
        {
            const Command cmd = Command.Playback | Command.Play;
            Assert.That(cmd.WithChannel(channelId).Channel(), Is.EqualTo(channelId));
        }
    }
}