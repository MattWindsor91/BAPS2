namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for unpacked commands that involve channels (playback or playlist commands).
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="PlaybackOp"/>) that this command type uses.</typeparam>
    public abstract class ChannelCommandBase<TOp> : CommandBase<TOp>
    {
        public byte ChannelId { get; }

        protected ChannelCommandBase(TOp op, byte channelId, bool modeFlag) : base(op, modeFlag)
        {
            ChannelId = channelId;
        }

        public override CommandWord Packed => OpAsCommandWord(Op).WithChannelModeFlag(ModeFlag).WithChannel(ChannelId);
    }
}