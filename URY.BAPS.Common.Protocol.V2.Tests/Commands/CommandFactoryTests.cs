using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for the command unpacking logic in <see cref="CommandFactory" />.
    /// </summary>
    public class CommandFactoryTests
    {
        private const CommandWord NonIndexedConfigTest = CommandWord.Config | CommandWord.SetConfigValue;

        private static readonly CommandWord IndexedConfigTest =
            (CommandWord.Config | CommandWord.SetConfigValue).WithConfigIndexedFlag(true).WithConfigIndex(5);

        private const CommandWord DatabaseTest =
            CommandWord.Database | CommandWord.GetShows;

        private static readonly CommandWord FlaggedDatabaseTest =
            (CommandWord.Database | CommandWord.GetShows).WithModeFlag(true);

        private static readonly CommandWord PlaybackTest =
            (CommandWord.Playback | CommandWord.Play).WithChannel(42);

        private static readonly CommandWord PlaylistTest =
            (CommandWord.Playlist | CommandWord.ResetPlaylist).WithChannel(63);

        private const CommandWord SystemTest = CommandWord.System | CommandWord.Quit;

        [UsedImplicitly]
        public static TheoryData<CommandWord> CommandWordRoundTripData =>
            new TheoryData<CommandWord>
            {
                NonIndexedConfigTest,
                IndexedConfigTest,
                DatabaseTest,
                FlaggedDatabaseTest,
                PlaybackTest,
                PlaylistTest,
                SystemTest
            };

        /// <summary>
        ///     Tests that unpacking, then packing, a command word preserves the original word.
        /// </summary>
        /// <param name="cmd">The packed word to test.</param>
        [Theory]
        [MemberData(nameof(CommandWordRoundTripData))]
        public void TestUnpack_RoundTrip(CommandWord cmd)
        {
            var unpacked = cmd.Unpack();
            var actualCmd = unpacked.Packed;
            Assert.Equal(cmd, actualCmd);
        }

        /// <summary>
        ///     Tests that a representative database word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_Database_Flagged()
        {
            var raw = FlaggedDatabaseTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<DatabaseCommand>(raw);
            Assert.Equal(DatabaseOp.GetShows, unpacked.Op);
            Assert.True(unpacked.ModeFlag);
        }

        /// <summary>
        ///     Tests that a representative indexed config word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_IndexedConfig()
        {
            var raw = IndexedConfigTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<IndexedConfigCommand>(raw);
            Assert.Equal(ConfigOp.SetConfigValue, unpacked.Op);
            Assert.Equal(5, unpacked.Index);
        }

        /// <summary>
        ///     Tests that a representative non-indexed config word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_NonIndexedConfig()
        {
            var raw = NonIndexedConfigTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<NonIndexedConfigCommand>(raw);
            Assert.Equal(ConfigOp.SetConfigValue, unpacked.Op);
        }

        /// <summary>
        ///     Tests that a representative playback word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_Playback()
        {
            var raw = PlaybackTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<PlaybackCommand>(raw);
            Assert.Equal(PlaybackOp.Play, unpacked.Op);
            Assert.Equal(42, unpacked.ChannelId);
            Assert.False(unpacked.ModeFlag);
        }

        /// <summary>
        ///     Tests that a representative playlist word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_Playlist()
        {
            var raw = PlaylistTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<PlaylistCommand>(raw);
            Assert.Equal(PlaylistOp.ResetPlaylist, unpacked.Op);
            Assert.Equal(63, unpacked.ChannelId);
            Assert.False(unpacked.ModeFlag);
        }

        /// <summary>
        ///     Tests that a representative system word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_System()
        {
            var raw = SystemTest.Unpack();
            var unpacked = Assert.IsAssignableFrom<SystemCommand>(raw);
            Assert.Equal(SystemOp.Quit, unpacked.Op);
            Assert.False(unpacked.ModeFlag);
        }
    }
}