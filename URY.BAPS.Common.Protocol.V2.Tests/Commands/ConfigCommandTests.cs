using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="NonIndexedConfigCommand" />.
    /// </summary>
    public class ConfigCommandTests
    {
        /// <summary>
        ///     Tests that a non-indexed non-flagged config command packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_ModeFlag()
        {
            var expected = ConfigOp.SetConfigValue.AsCommandWord().WithModeFlag(true);

            var unpacked = new NonIndexedConfigCommand(ConfigOp.SetConfigValue, true);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Tests that a non-indexed non-flagged config command packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_NoModeFlag()
        {
            var expected = ConfigOp.SetConfigValue.AsCommandWord();

            var unpacked = new NonIndexedConfigCommand(ConfigOp.SetConfigValue, false);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}