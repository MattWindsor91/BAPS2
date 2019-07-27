using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class PlaybackCommand : ChannelCommandBase<PlaybackOp>
    {
        public PlaybackCommand(PlaybackOp op, byte channelId, bool modeFlag = false) : base(op, channelId, modeFlag)
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