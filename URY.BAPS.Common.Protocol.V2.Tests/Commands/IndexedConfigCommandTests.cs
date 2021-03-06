﻿using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Commands
{
    /// <summary>
    ///     Tests for <see cref="IndexedConfigCommand" />.
    /// </summary>
    public class IndexedConfigCommandTests
    {
        /// <summary>
        ///     Tests that an indexed config command without mode flag packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_ModeFlag()
        {
            var expected = (ushort) (
                CommandGroup.Config.ToWordBits()
            | ConfigOp.SetConfigValue.ToWordBits()
                    | CommandMasks.ConfigIndexedFlag
                    | CommandMasks.ModeFlag
                        | CommandPacking.ConfigIndex(5));

            var unpacked = new IndexedConfigCommand(ConfigOp.SetConfigValue, 5, true);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Tests that an indexed config command without mode flag packs correctly.
        /// </summary>
        [Fact]
        public void TestPacked_NoModeFlag()
        {
            var expected = (ushort) (
                CommandGroup.Config.ToWordBits()
            | ConfigOp.SetConfigValue.ToWordBits()
            | CommandMasks.ConfigIndexedFlag
            | CommandPacking.ConfigIndex(5));

            var unpacked = new IndexedConfigCommand(ConfigOp.SetConfigValue, 5, false);
            var actual = unpacked.Packed;

            Assert.Equal(expected, actual);
        }
    }
}