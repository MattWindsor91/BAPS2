using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

using static URY.BAPS.Common.Protocol.V2.Commands.CommandFactory;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for the command unpacking logic in <see cref="CommandFactory" />.
    /// </summary>
    public class CommandFactoryTests
    {
        private static readonly ushort NonIndexedConfigTest =
            (ushort) (CommandGroup.Config.ToWordBits() | ConfigOp.SetConfigValue.ToWordBits());

        private static readonly ushort IndexedConfigTest =
            (ushort) (CommandGroup.Config.ToWordBits() | ConfigOp.SetConfigValue.ToWordBits() | CommandMasks.ConfigIndexedFlag | CommandPacking.ConfigIndex(5));

        private static readonly ushort DatabaseTest =
            (ushort) (CommandGroup.Database.ToWordBits() | DatabaseOp.GetShows.ToWordBits());

        private static readonly ushort FlaggedDatabaseTest =
            (ushort) (CommandGroup.Database.ToWordBits() | DatabaseOp.GetShows.ToWordBits() | CommandMasks.ModeFlag);

        private static readonly ushort PlaybackTest =
            (ushort) (CommandGroup.Playback.ToWordBits() | PlaybackOp.Play.ToWordBits() | CommandPacking.Channel(42));

        private static readonly ushort PlaylistTest =
            (ushort) (CommandGroup.Playlist.ToWordBits() | PlaylistOp.ResetPlaylist.ToWordBits() | CommandPacking.Channel(63));

        private static readonly ushort SystemTest = (ushort) (CommandGroup.System.ToWordBits() | SystemOp.Quit.ToWordBits());

        [UsedImplicitly]
        public static TheoryData<ushort> CommandWordRoundTripData =>
            new TheoryData<ushort>
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
        public void TestUnpack_RoundTrip(ushort cmd)
        {
            var unpacked = Unpack(cmd);
            var actualCmd = unpacked.Packed;
            Assert.Equal(cmd, actualCmd);
        }

        /// <summary>
        ///     Tests that a representative database word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_Database_Flagged()
        {
            var raw = Unpack(FlaggedDatabaseTest);
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
            var raw = Unpack(IndexedConfigTest);
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
            var raw = Unpack(NonIndexedConfigTest);
            var unpacked = Assert.IsAssignableFrom<NonIndexedConfigCommand>(raw);
            Assert.Equal(ConfigOp.SetConfigValue, unpacked.Op);
        }

        /// <summary>
        ///     Tests that a representative playback word unpacks properly.
        /// </summary>
        [Fact]
        public void TestUnpack_Playback()
        {
            var raw = Unpack(PlaybackTest);
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
            var raw = Unpack(PlaylistTest);
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
            var raw = Unpack(SystemTest);
            var unpacked = Assert.IsAssignableFrom<SystemCommand>(raw);
            Assert.Equal(SystemOp.Quit, unpacked.Op);
            Assert.False(unpacked.ModeFlag);
        }
    }
}