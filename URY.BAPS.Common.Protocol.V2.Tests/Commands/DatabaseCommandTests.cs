﻿using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="DatabaseCommand" />.
    /// </summary>
    public class DatabaseCommandTests
    {
        /// <summary>
        ///     Tests that a playback command with a flag packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_Flag()
        {
            var expected = (ushort) (CommandGroup.Database.ToWordBits() | DatabaseOp.GetShows.ToWordBits() |
                                     CommandMasks.ModeFlag);

            var unpacked = new DatabaseCommand(DatabaseOp.GetShows, 0, true);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}