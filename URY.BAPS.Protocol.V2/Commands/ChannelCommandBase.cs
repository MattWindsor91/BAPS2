namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Abstract base class for unpacked commands that involve channels (playback or playlist commands).
    /// </summary>
    /// <typeparam name="TOp">The op enum (eg <see cref="PlaybackOp"/>) that this command type uses.</typeparam>
    public abstract class ChannelCommandBase<TOp> : CommandBase<TOp>
    {
        public byte Channel { get; }
        public bool ModeFlag { get; }

        protected ChannelCommandBase(TOp op, byte channel, bool modeFlag) : base(op)
        {
            Channel = channel;
            ModeFlag = modeFlag;
        }

        public override CommandWord Packed => OpAsCommandWord(Op).WithChannelModeFlag(ModeFlag).WithChannel(Channel);
    }
}