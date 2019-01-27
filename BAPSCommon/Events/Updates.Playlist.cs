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
        public delegate void ChannelStateEventHandler(object sender, ChannelStateEventArgs e);

        public delegate void ItemAddEventHandler(object sender, ItemAddEventArgs e);

        public delegate void ItemDeleteEventHandler(object sender, ItemDeleteEventArgs e);

        public delegate void ItemMoveEventHandler(object sender, ItemMoveEventArgs e);

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
        public class ItemAddEventArgs : ItemEventArgs
        {
            public ItemAddEventArgs(ushort channelId, uint index, Track item)
                : base(channelId, index)
            {
                Item = item;
            }

            public Track Item { get; }
        }

        /// <summary>
        ///     Payload for a track-list item move server update.
        /// </summary>
        public class ItemMoveEventArgs : ItemEventArgs
        {
            public ItemMoveEventArgs(ushort channelId, uint fromIndex, uint toIndex)
                : base(channelId, fromIndex)
            {
                NewIndex = toIndex;
            }

            public uint NewIndex { get; }
        }

        /// <summary>
        ///     Payload for a track-list item delete server update.
        /// </summary>
        public class ItemDeleteEventArgs : ItemEventArgs
        {
            public ItemDeleteEventArgs(ushort channelId, uint index) : base(channelId, index)
            {
            }
        }
    }
}