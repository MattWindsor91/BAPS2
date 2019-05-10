using URY.BAPS.Model.MessageEvents;
using URY.BAPS.Model.Playback;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Model;

namespace URY.BAPS.Protocol.V2.Decode
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
                    ReportMalformedCommand(CommandGroup.Playback);
                    break;
            }
        }

        
        private void DecodeLoad(ushort channelId)
        {
            var index = ReceiveUint();
            var type = DecodeTrackType();
            var description = ReceiveString();

            var duration = 0U;
            if (type.HasAudio()) duration = ReceiveUint();

            var text = "";
            if (type.HasText()) text = ReceiveString();

            var track = TrackFactory.Create(type, description, duration, text);

            Dispatch(new MarkerChangeArgs(channelId, MarkerType.Position, 0U));
            Dispatch(new TrackLoadArgs(channelId, index, track));
        }
        
        private void DecodeMarkerChange(byte channelId, MarkerType markerType)
        {
            var position = ReceiveUint();
            Dispatch(new MarkerChangeArgs(channelId, markerType, position));
        }

        private void DecodePlaybackStateChange(byte channelId, PlaybackState state)
        {
            Dispatch(new PlaybackStateChangeArgs(channelId, state));
        }
    }
}