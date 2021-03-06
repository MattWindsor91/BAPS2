﻿using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="SystemCommand" />.
    /// </summary>
    public class SystemCommandTests
    {
        /// <summary>
        ///     Tests that a system command with no arguments packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_NullaryCommand()
        {
            var expected = (ushort) (CommandGroup.System.ToWordBits() | SystemOp.Quit.ToWordBits());

            var unpacked = new SystemCommand(SystemOp.Quit);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}