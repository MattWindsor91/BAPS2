using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    public class PlaylistCommand : ChannelCommand<PlaylistOp>
    {
        public PlaylistCommand(PlaylistOp op, byte channelId, bool modeFlag = false) : base(op, channelId, modeFlag)
        {
        }

        public override void Accept(ICommandVisitor? visitor)
        {
            visitor?.Visit(this);
        }

        protected override CommandGroup Group => CommandGroup.Playlist;
        protected override byte OpByte => (byte) Op;
    }
}