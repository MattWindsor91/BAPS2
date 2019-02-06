using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <summary>
        ///     Payload for a channel marker (position/cue/intro) server update.
        /// </summary>
        public class MarkerEventArgs : Events.MarkerEventArgs
        {
            /// <summary>
            ///     Constructs a channel marker server update.
            /// </summary>
            /// <param name="channelId">The ID of the channel whose marker is being moved.</param>
            /// <param name="marker">The marker being moved.</param>
            /// <param name="newValue">The new value of the marker.</param>
            public MarkerEventArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId, marker,
                newValue)
            {
            }
        }

        /// <summary>
        ///     Payload for a channel state (play/pause/stop) server update.
        /// </summary>
        public class PlayerStateEventArgs : ChannelEventArgs
        {
            public PlayerStateEventArgs(ushort channelId, PlaybackState state) : base(channelId)
            {
                State = state;
            }

            /// <summary>
            ///     The new state of the channel.
            /// </summary>
            public PlaybackState State { get; }
        }

        /// <summary>
        ///     Payload for a server update mentioning a change in the
        ///     loaded track of a channel.
        /// </summary>
        public class TrackLoadEventArgs : TrackEventArgs
        {
            public TrackLoadEventArgs(ushort channelId, uint index, Track track) : base(channelId, index, track)
            {
            }
        }
    }
}