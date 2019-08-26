using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for commands that involve channels (playback or playlist commands).
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="PlaybackOp" />) that this command type uses.</typeparam>
    public abstract class ChannelCommand<TOp> : Command<TOp>
    {
        protected ChannelCommand(TOp op, byte channelId, bool modeFlag) : base(op, modeFlag)
        {
            ChannelId = channelId;
        }

        public byte ChannelId { get; }

        protected override ushort CommandWordOp => CommandPacking.ChannelOp(OpByte);

        /// <summary>
        ///     The result of casting <see cref="Command{TOp}.Op"/> to a byte.
        /// </summary>
        protected abstract byte OpByte { get; }

        protected override ushort CommandWordFlags {
            get
            {
                var word = CommandPacking.Channel(ChannelId);
                if (ModeFlag) word |= CommandMasks.ChannelModeFlag;
                return word;
            }
        }

        public override string ToString()
        {
            var modePrefix = ModeFlag ? "!" : "";
            return $"{modePrefix}{Op}({ChannelId})";
        }
    }
}