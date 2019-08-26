using System;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Low-level helpers for packing parts of a command into a 16-bit
    ///     command word.
    /// </summary>
    public static class CommandPacking
    {
        #region Extension methods

        /// <summary>
        ///     Packs a <see cref="CommandGroup" /> into its BapsNet representation.
        /// </summary>
        /// <param name="group">The <see cref="CommandGroup" /> to pack.</param>
        /// <returns>The bit-pattern of <paramref name="group" /> in a packed BapsNet command word.</returns>
        public static ushort ToWordBits(this CommandGroup group)
        {
            return (ushort) ((ushort) group << CommandShifts.Group);
        }

        /// <summary>
        ///     Packs this <see cref="DatabaseOp"/> into its BapsNet representation
        /// </summary>
        /// <param name="op">The opcode to pack.</param>
        /// <returns>
        ///     The bit-pattern of <see cref="op"/> in a packed BapsNet command word,
        ///     less its group code.
        /// </returns>
        public static ushort ToWordBits(this DatabaseOp op)
        {
            return Op((byte) op);
        }

        /// <summary>
        ///     Packs this <see cref="ConfigOp"/> into its BapsNet representation
        /// </summary>
        /// <param name="op">The opcode to pack.</param>
        /// <returns>
        ///     The bit-pattern of <see cref="op"/> in a packed BapsNet command word,
        ///     less its group code.
        /// </returns>
        public static ushort ToWordBits(this ConfigOp op)
        {
            return Op((byte)op);
        }

        /// <summary>
        ///     Packs this <see cref="SystemOp"/> into its BapsNet representation
        /// </summary>
        /// <param name="op">The opcode to pack.</param>
        /// <returns>
        ///     The bit-pattern of <see cref="op"/> in a packed BapsNet command word,
        ///     less its group code.
        /// </returns>
        public static ushort ToWordBits(this SystemOp op)
        {
            return Op((byte)op);
        }

        /// <summary>
        ///     Packs this <see cref="PlaybackOp"/> into its BapsNet representation
        /// </summary>
        /// <param name="op">The opcode to pack.</param>
        /// <returns>
        ///     The bit-pattern of <see cref="op"/> in a packed BapsNet command word,
        ///     less its group code.
        /// </returns>
        public static ushort ToWordBits(this PlaybackOp op)
        {
            return ChannelOp((byte)op);
        }

        /// <summary>
        ///     Packs this <see cref="PlaylistOp"/> into its BapsNet representation
        /// </summary>
        /// <param name="op">The opcode to pack.</param>
        /// <returns>
        ///     The bit-pattern of <see cref="op"/> in a packed BapsNet command word,
        ///     less its group code.
        /// </returns>
        public static ushort ToWordBits(this PlaylistOp op)
        {
            return ChannelOp((byte)op);
        }

        #endregion Extension methods

        #region Command opcodes

        public static ushort ChannelOp(byte op)
        {
            return (ushort)((op << CommandShifts.ChannelOp) & CommandMasks.ChannelOp);
        }

        public static ushort Op(byte op)
        {
            return (ushort)((op << CommandShifts.Op) & CommandMasks.Op);
        }

        #endregion Command opcodes

        #region Channel command parts

        /// <summary>
        ///     Packs a channel ID ready for OR-ing into a command word.
        /// </summary>
        /// <param name="channelId">The channel ID to pack.</param>
        /// <returns>
        ///     The packed equivalent of <paramref name="channelId" />.
        ///     Raises an exception if <paramref name="channelId" /> doesn't fit
        ///     inside a BapsNet channel command.
        /// </returns>
        public static ushort Channel(byte channelId)
        {
            var bytes = (ushort) channelId;
            if ((bytes & CommandMasks.ChannelId) != bytes)
                throw new ArgumentOutOfRangeException(nameof(channelId),
                    "Channel ID doesn't fit in the channel ID bitfield.");
            return bytes;
        }

        #endregion Channel command parts

        #region Non-channel command parts

        /// <summary>
        ///     Packs a value ready for OR-ing into a command word.
        /// </summary>
        /// <param name="value">The command value to pack.</param>
        /// <returns>
        ///     The packed equivalent of <paramref name="value" />.
        ///     Raises an exception if <paramref name="value" /> doesn't fit
        ///     inside a BapsNet channel command.
        /// </returns>
        public static ushort Value(byte value)
        {
            return ValueLike(value, CommandMasks.Value, "value");
        }

        /// <summary>
        ///     Constructs the lower part of a 'normal' command word from its
        ///     mode flag and value.
        /// </summary>
        /// <param name="modeFlag">The mode flag to put into the command word.</param>
        /// <param name="value">The value to put into the command word.</param>
        /// <returns>
        ///     A partial command word that, when OR-ed together with the
        ///     appropriate opcode, forms a normal (or value-based config) command
        ///     word.
        /// </returns>
        public static ushort NormalCommandFlags(bool modeFlag, byte value)
        {
            var word = Value(value);
            if (modeFlag) word |= CommandMasks.ModeFlag;
            return word;
        }

        #endregion Non-channel command parts

        #region Config indices

        /// <summary>
        ///     Packs a config index ready for OR-ing into a command word.
        /// </summary>
        /// <param name="index">The config index to pack.</param>
        /// <returns>
        ///     The packed equivalent of <paramref name="index" />.
        ///     Raises an exception if <paramref name="index" /> doesn't fit
        ///     inside a BapsNet channel command.
        /// </returns>
        public static ushort ConfigIndex(byte index)
        {
            return ValueLike(index, CommandMasks.ConfigIndex, "config index");
        }

        #endregion Config indices

        private static ushort ValueLike(byte value, ushort mask, string nameForException)
        {
            var bytes = (ushort)value;
            if ((bytes & mask) != bytes)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"The {nameForException} ID didn't fit in its allotted bitfield.");
            return bytes;
        }
    }
}