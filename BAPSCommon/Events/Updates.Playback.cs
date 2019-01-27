using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <summary>
        ///     Event handler for channel marker server updates.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The payload of this event.</param>
        public delegate void ChannelMarkerEventHandler(object sender, ChannelMarkerEventArgs e);

        /// <summary>
        ///     Payload for a channel marker (position/cue/intro) server update.
        /// </summary>
        public class ChannelMarkerEventArgs : Events.ChannelMarkerEventArgs
        {
            /// <summary>
            ///     Constructs a channel marker server update.
            /// </summary>
            /// <param name="channelId">The ID of the channel whose marker is being moved.</param>
            /// <param name="marker">The marker being moved.</param>
            /// <param name="newValue">The new value of the marker.</param>
            public ChannelMarkerEventArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId, marker, newValue)
            {
            }
        }

        /// <summary>
        ///     Payload for a channel state (play/pause/stop) server update.
        /// </summary>
        public class ChannelStateEventArgs : ChannelEventArgs
        {
            public ChannelStateEventArgs(ushort channelId, ChannelState state) : base(channelId)
            {
                State = state;
            }

            /// <summary>
            ///     The new state of the channel.
            /// </summary>
            public ChannelState State { get; }
        }
    }
}