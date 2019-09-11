using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    public partial class CommandDecoder
    {
        public void Visit(PlaylistCommand command)
        {
            switch (command.Op)
            {
                case PlaylistOp.Item when command.ModeFlag:
                    DecodeItem(command.ChannelId);
                    break;
                case PlaylistOp.Item when !command.ModeFlag:
                    DecodeCount(CountType.PlaylistItem, command.ChannelId);
                    break;
                case PlaylistOp.MoveItemTo:
                    DecodeMoveItemTo(command.ChannelId);
                    break;
                case PlaylistOp.DeleteItem:
                    DecodeDeleteItem(command.ChannelId);
                    break;
                case PlaylistOp.ResetPlaylist:
                    DecodeResetPlaylist(command.ChannelId);
                    break;
                default:
                    ReportMalformedCommand(CommandGroup.Playlist);
                    break;
            }
        }

        private void DecodeResetPlaylist(byte channelId)
        {
            Dispatch(new PlaylistResetArgs(channelId));
        }

        private void DecodeDeleteItem(byte channelId)
        {
            var index = ReceiveUint();
            Dispatch(new TrackDeleteArgs(channelId, index));
        }

        private void DecodeMoveItemTo(byte channelId)
        {
            var indexFrom = ReceiveUint();
            var indexTo = ReceiveUint();
            Dispatch(new TrackMoveArgs(channelId, indexFrom, indexTo));
        }

        #region Implemented differently for clients and servers

        protected abstract void DecodeItem(byte channelId);

        #endregion Implemented differently for clients and servers
    }
}