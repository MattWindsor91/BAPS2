namespace URY.BAPS.Protocol.V2.Commands
{
    public class PlaybackCommand : ChannelCommandBase<PlaybackOp>
    {
        public PlaybackCommand(PlaybackOp op, byte channel, bool modeFlag = false) : base(op, channel, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(PlaybackOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}