using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class PlaybackCommand : ChannelCommand<PlaybackOp>
    {
        public PlaybackCommand(PlaybackOp op, byte channelId, bool modeFlag = false) : base(op, channelId, modeFlag)
        {
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        protected override CommandGroup Group => CommandGroup.Playback;
        protected override byte OpByte => (byte)Op;
    }
}