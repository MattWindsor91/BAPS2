namespace URY.BAPS.Protocol.V2.Commands
{
    public class PlaylistCommand : ChannelCommandBase<PlaylistOp>
    {
        public PlaylistCommand(PlaylistOp op, byte channel, bool modeFlag = false) : base(op, channel, modeFlag)
        {
        }

        protected override CommandWord OpAsCommandWord(PlaylistOp op)
        {
            return op.AsCommandWord();
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }
    }
}