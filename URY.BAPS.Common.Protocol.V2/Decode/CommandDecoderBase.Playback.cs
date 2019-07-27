using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Model;
using URY.BAPS.Common.Protocol.V2.Ops;

namespace URY.BAPS.Common.Protocol.V2.Decode
{
    public partial class CommandDecoderBase
    {
        public void Visit(PlaybackCommand command)
        {
            var channelId = command.ChannelId;

            switch (command.Op)
            {
                case PlaybackOp.Play:
                case PlaybackOp.Pause:
                case PlaybackOp.Stop:
                    DecodePlaybackStateChange(channelId, command.Op.AsPlaybackState());
                    break;
                case PlaybackOp.Volume:
                    DecodeVolume();
                    break;
                case PlaybackOp.Load:
                    DecodeLoad(channelId);
                    break;
                case PlaybackOp.Position when command.ModeFlag:
                case PlaybackOp.CuePosition when command.ModeFlag:
                case PlaybackOp.IntroPosition when command.ModeFlag:
                    DecodeMarkerGet(channelId, command.Op.AsMarkerType());
                    break;
                case PlaybackOp.Position when !command.ModeFlag:
                case PlaybackOp.CuePosition when !command.ModeFlag:
                case PlaybackOp.IntroPosition when !command.ModeFlag:
                    DecodeMarkerChange(channelId, command.Op.AsMarkerType());
                    break;
                default:
                    ReportMalformedCommand(CommandGroup.Playback);
                    break;
            }
        }

        private void DecodePlaybackStateChange(byte channelId, PlaybackState state)
        {
            Dispatch(new PlaybackStateChangeArgs(channelId, state));
        }

        private void DecodeVolume()
        {
            // Deliberately ignore
            _ = ReceiveFloat();
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

        private void DecodeMarkerGet(byte channelId, MarkerType markerType)
        {
            Dispatch(new MarkerGetArgs(channelId, markerType));
        }

        private void DecodeMarkerChange(byte channelId, MarkerType markerType)
        {
            var position = ReceiveUint();
            Dispatch(new MarkerChangeArgs(channelId, markerType, position));
        }
    }
}