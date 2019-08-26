using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public static class CommandMasks
    {
        /*
         * There are three different shapes that a command word can take:
         *
         * |GROUP|OPERATION|M|VALUE--------| <-- 'normal'
         * |GROUP|OPERATION--|M|CHANNELID--| <-- 'channel'
         * |GROUP|OPERATION|M|X|INDEX------| <-- 'config'
         * [F|D|E|C|B|A|9|8|7|6|5|4|3|2|1|0]
         *
         * (M = mode flag; X = is-indexed flag).
         *
         * Some config commands don't have an index, but instead use the six
         * lowest bits as a value.  We mask them like normal 'indexed'
         * config commands, but need to make sure that we ignore the
         * is-indexed flag.
         */

        /// <summary>
        ///     Mask used, alongside a shift of 13, to select the
        ///     <see cref="CommandGroup" /> part of a command word.
        /// </summary>
        public const ushort Group = 0b11100000_00000000;

        /// <summary>
        ///     Mask used, alongside a shift of 7, to select the
        ///     <see cref="PlaybackOp" /> or
        ///     <see cref="PlaylistOp" /> part of a command word.
        /// </summary>
        public const ushort ChannelOp = 0b00011111_10000000;

        /// <summary>
        ///     Mask used to select the mode flag of a channel command word.
        /// </summary>
        public const ushort ChannelModeFlag = 0b00000000_01000000;

        /// <summary>
        ///     Mask used to select the operation of a non-channel command word.
        /// </summary>
        public const ushort Op = 0b00011111_00000000;

        /// <summary>
        ///     Mask used to select the channel ID of a channel command word.
        /// </summary>
        public const ushort ChannelId = 0b00000000_00111111;

        /// <summary>
        ///     Mask used to select modes on database, config, and system command words.
        /// </summary>
        public const ushort ModeFlag = 0b00000000_10000000;

        /// <summary>
        ///     Mask used to select values on database and system command words.
        /// </summary>
        public const ushort Value = 0b00000000_01111111;

        /// <summary>
        ///     Mask used to select the 'has index' flag on config words.
        /// </summary>
        public const ushort ConfigIndexedFlag = 0b00000000_01000000;

        /// <summary>
        ///     Mask used to select indexes on config command words.
        /// </summary>
        public const ushort ConfigIndex = 0b00000000_00111111;
    }
}