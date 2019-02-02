using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        public delegate void ChannelResetEventHandler(object sender, ChannelResetEventArgs e);

        /// <summary>
        ///     Event handler for channel state server updates.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The payload of this event.</param>
        public delegate void ChannelStateEventHandler(object sender, PlayerStateEventArgs e);

        public delegate void ItemAddEventHandler(object sender, TrackAddEventArgs e);

        public delegate void ItemDeleteEventHandler(object sender, TrackDeleteEventArgs e);

        public delegate void ItemMoveEventHandler(object sender, TrackMoveEventArgs e);

        /// <summary>
        ///     Payload for a channel track-list reset server update.
        /// </summary>
        public class ChannelResetEventArgs : ChannelEventArgs
        {
            public ChannelResetEventArgs(ushort channelId) : base(channelId)
            {
            }
        }

        /// <summary>
        ///     Payload for a track-list item add server update.
        /// </summary>
        public class TrackAddEventArgs : TrackIndexEventArgs
        {
            public TrackAddEventArgs(ushort channelId, uint index, Track item)
                : base(channelId, index)
            {
                Item = item;
            }

            public Track Item { get; }
        }

        /// <summary>
        ///     Payload for a track-list item move server update.
        /// </summary>
        public class TrackMoveEventArgs : TrackIndexEventArgs
        {
            public TrackMoveEventArgs(ushort channelId, uint fromIndex, uint toIndex)
                : base(channelId, fromIndex)
            {
                NewIndex = toIndex;
            }

            public uint NewIndex { get; }
        }

        /// <summary>
        ///     Payload for a track-list item delete server update.
        /// </summary>
        public class TrackDeleteEventArgs : TrackIndexEventArgs
        {
            public TrackDeleteEventArgs(ushort channelId, uint index) : base(channelId, index)
            {
            }
        }
    }
}