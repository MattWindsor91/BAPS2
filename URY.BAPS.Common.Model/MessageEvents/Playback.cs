using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Payload for a server update mentioning a change in the
    ///     loaded track of a channel.
    /// </summary>
    public class TrackLoadArgs : TrackIndexArgsBase
    {
        public TrackLoadArgs(ushort channelId, uint index, ITrack track) : base(channelId, index)
        {
            Track = track;
        }

        /// <summary>
        ///     The track being mentioned in the event.
        /// </summary>
        public ITrack Track { get; }
    }

    /// <summary>
    ///     Payload for a channel state (play/pause/stop) change message.
    /// </summary>
    public class PlaybackStateChangeArgs : ChannelArgsBase
    {
        public PlaybackStateChangeArgs(ushort channelId, PlaybackState state) : base(channelId)
        {
            State = state;
        }

        /// <summary>
        ///     The new state of the channel.
        /// </summary>
        public PlaybackState State { get; }

        public override string ToString()
        {
            return $"PlaybackStateChange: channel {ChannelId} is {State}";
        }
    }
}