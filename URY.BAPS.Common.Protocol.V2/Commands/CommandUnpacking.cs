using System;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Low-level helpers for unpacking parts of a 16-bit command word.
    /// </summary>
    public static class CommandUnpacking
    {
        public static CommandGroup Group(ushort cmd)
        {
            return (CommandGroup) ((cmd & CommandMasks.Group) >> CommandShifts.Group);
        }

        #region Command opcodes

        public static PlaybackOp PlaybackOp(ushort cmd)
        {
            return (PlaybackOp) CheckOpInEnum<PlaybackOp>(ChannelOp(cmd), "playback");
        }

        public static PlaylistOp PlaylistOp(ushort cmd)
        {
            return (PlaylistOp) CheckOpInEnum<PlaylistOp>(ChannelOp(cmd), "playlist");
        }

        public static DatabaseOp DatabaseOp(ushort cmd)
        {
            return (DatabaseOp) CheckOpInEnum<DatabaseOp>(Op(cmd), "database");
        }

        public static ConfigOp ConfigOp(ushort cmd)
        {
            return (ConfigOp) CheckOpInEnum<ConfigOp>(Op(cmd), "config");
        }

        public static SystemOp SystemOp(ushort cmd)
        {
            return (SystemOp) CheckOpInEnum<SystemOp>(Op(cmd), "system");
        }

        private static byte CheckOpInEnum<T>(byte op, string enumName)
        {
            if (!Enum.IsDefined(typeof(T), op))
                throw new ArgumentOutOfRangeException(nameof(op), op, $"Not a valid {enumName} operation");
            // Ideally, this would also cast op to T, but that isn't valid C#:
            // there is no way we can express that T is convertible from byte,
            // at least without doing weird reflection stuff.
            return op;
        }

        #endregion Command opcodes

        #region Channel command parts

        /// <summary>
        ///     Tests whether a packed command word has the channel mode flag set.
        /// </summary>
        /// <param name="cmd">The command word to test.</param>
        /// <returns>Whether <paramref name="cmd" /> has the channel mode flag.</returns>
        public static bool HasChannelModeFlag(ushort cmd)
        {
            return (cmd & CommandMasks.ChannelModeFlag) != 0;
        }

        /// <summary>
        ///     Extracts the operation part of a packed channel command word.
        /// </summary>
        /// <param name="cmd">The command word to query.</param>
        /// <returns>
        ///     The raw operation code, masked and shifted out of <paramref name="cmd" />
        ///     using the channel op masks and shifts.
        /// </returns>
        public static byte ChannelOp(ushort cmd)
        {
            return (byte) ((cmd & CommandMasks.ChannelOp) >> CommandShifts.ChannelOp);
        }

        /// <summary>
        ///     Returns the channel component of a packed BapsNet playback or playlist command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a channel component---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The channel component of <see cref="cmd" />.</returns>
        public static byte Channel(ushort cmd)
        {
            return (byte) (cmd & CommandMasks.ChannelId);
        }

        #endregion Channel command parts

        #region Non-channel command parts

        /// <summary>
        ///     Extracts the operation part of a database, config or system command word.
        /// </summary>
        /// <param name="cmd">The command word to query.</param>
        /// <returns>
        ///     The raw operation code, masked and shifted out of <paramref name="cmd" />
        ///     using the normal masks and shifts.
        /// </returns>
        public static byte Op(ushort cmd)
        {
            return (byte) ((cmd & CommandMasks.Op) >> CommandShifts.Op);
        }

        /// <summary>
        ///     Returns the value component of a BapsNet database or system command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a value---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The channel component of <see cref="cmd" />.</returns>
        public static byte Value(ushort cmd)
        {
            return (byte) (cmd & CommandMasks.Value);
        }

        /// <summary>
        ///     Tests whether a packed command word has the (database/config/system) mode flag set.
        /// </summary>
        /// <param name="cmd">The command word to test.</param>
        /// <returns>Whether <paramref name="cmd" /> has the mode flag.</returns>
        public static bool HasModeFlag(ushort cmd)
        {
            return (cmd & CommandMasks.ModeFlag) != 0;
        }

        #endregion Non-channel command parts

        #region Config indices

        /// <summary>
        ///     Tests whether a packed command word has the config indexed flag set.
        /// </summary>
        /// <param name="cmd">The command word to test.</param>
        /// <returns>Whether <paramref name="cmd" /> has the config indexed flag.</returns>
        public static bool HasConfigIndexedFlag(ushort cmd)
        {
            return (cmd & CommandMasks.ConfigIndexedFlag) != 0;
        }

        /// <summary>
        ///     Returns the index component of a BapsNet config command word.
        ///     <para>
        ///         This doesn't perform any validation on the command word to see whether it actually
        ///         has a index---for command words without one, the result is undefined.
        ///     </para>
        /// </summary>
        /// <param name="cmd">The command word to mask-off.</param>
        /// <returns>The index component of <see cref="cmd" />.</returns>
        public static byte ConfigIndex(ushort cmd)
        {
            return (byte) (cmd & CommandMasks.ConfigIndex);
        }

        #endregion Config indices
    }
}