﻿using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Updates
    {
        /// <summary>
        ///     Payload for a channel track-list reset server update.
        /// </summary>
        public class PlaylistResetEventArgs : ChannelEventArgs
        {
            public PlaylistResetEventArgs(ushort channelId) : base(channelId)
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