using URY.BAPS.Protocol.V2.Commands;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="ConfigCommand"/>.
    /// </summary>
    public class ConfigCommandTests
    {
        /// <summary>
        ///     Tests that a non-indexed config command packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked()
        {
            var expected = ConfigOp.SetConfigValue.AsCommandWord();

            var unpacked = new ConfigCommand(ConfigOp.SetConfigValue);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}
