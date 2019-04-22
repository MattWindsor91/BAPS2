using System;
using System.Collections.Generic;
using System.Text;
using URY.BAPS.Protocol.V2.Commands;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="IndexedConfigCommand"/>.
    /// </summary>
    public class IndexedConfigCommandTests
    {
        /// <summary>
        ///     Tests that an indexed config command packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked()
        {
            var expected = ConfigOp.SetConfigValue.AsCommandWord().WithConfigIndexedFlag(true).WithConfigIndex(5);

            var unpacked = new IndexedConfigCommand(ConfigOp.SetConfigValue, 5);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}
