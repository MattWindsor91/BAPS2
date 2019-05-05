using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common
{
    public partial class CommandDecoder
    {
        public void Visit(PlaybackCommand command)
        {
            var channelId = command.ChannelId;
            switch (command.Op)
            {
                case PlaybackOp.Play:
                case PlaybackOp.Pause:
                case PlaybackOp.Stop:
                {
                    DecodePlaybackStateChange(channelId, command.Op.AsPlaybackState());
                }
                    break;
                case PlaybackOp.Volume:
                {
                    // Deliberately ignore
                    _ = ReceiveFloat();
                }
                    break;
                case PlaybackOp.Load:
                {
                    DecodeLoad(channelId);
                }
                    break;
                case PlaybackOp.Position:
                case PlaybackOp.CuePosition:
                case PlaybackOp.IntroPosition:
                {
                    DecodeMarkerChange(channelId, command.Op.AsMarkerType());
                }
                    break;
                default:
                    _receiver.OnUnknownCommand(command.Packed, "possibly a malformed PLAYBACK");
                    break;
            }
        }

        private void DecodeLoad(ushort channelId)
        {
            var index = ReceiveUint();
            var type = (TrackType) ReceiveUint();
            var description = ReceiveString();

            var duration = 0U;
            if (type.HasAudio()) duration = ReceiveUint();

            var text = "";
            if (type.HasText()) text = ReceiveString();

            var track = TrackFactory.Create(type, description, duration, text);

            _receiver.OnMarkerChange(new MarkerChangeArgs(channelId, MarkerType.Position, 0U));
            _receiver.OnLoadedItem(new TrackLoadArgs(channelId, index, track));
        }

        private void DecodeMarkerChange(byte channelId, MarkerType markerType)
        {
            var position = ReceiveUint();
            _receiver.OnMarkerChange(new MarkerChangeArgs(channelId, markerType, position));
        }

        private void DecodePlaybackStateChange(byte channelId, PlaybackState state)
        {
            _receiver.OnPlaybackStateChange(new PlaybackStateChangeArgs(channelId, state));
        }
    }
}