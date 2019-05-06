using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
{
    public partial class CommandDecoder
    {
        public void Visit(PlaylistCommand command)
        {
            switch (command.Op)
            {
                case PlaylistOp.Item:
                    if (command.ModeFlag)
                    {
                        DecodeItem(command.ChannelId);
                    }
                    else
                    {
                        // Deliberately ignore?
                        _ = ReceiveUint();
                    }

                    break;
                case PlaylistOp.MoveItemTo:
                {
                    DecodeMoveItemTo(command.ChannelId);
                }
                    break;
                case PlaylistOp.DeleteItem:
                {
                    DecodeDeleteItem(command.ChannelId);
                }
                    break;
                case PlaylistOp.ResetPlaylist:
                {
                    DecodeResetPlaylist(command.ChannelId);
                }
                    break;
                default:
                    ReportMalformedCommand(command, CommandGroup.Playlist);
                    break;
            }
        }

        private void DecodeItem(byte channelId)
        {
            var index = ReceiveUint();
            var type = (TrackType) ReceiveUint();
            var description = ReceiveString();
            var entry = TrackFactory.Create(type, description);
            Dispatch(new TrackAddArgs(channelId, index, entry));
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
    }
}