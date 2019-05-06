﻿using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Events
{
    /// <summary>
    ///     Payload for a server update mentioning a change in the
    ///     loaded track of a channel.
    /// </summary>
    public class TrackLoadArgs : TrackIndexArgsBase
    {
        public TrackLoadArgs(ushort channelId, uint index, Track track) : base(channelId, index)
        {
            Track = track;
        }

        /// <summary>
        ///     The track being mentioned in the event.
        /// </summary>
        public Track Track { get; }
    }

    /// <summary>
    ///     Payload for a channel state (play/pause/stop) server update.
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
    }

    /// <summary>
    ///     Payload for a channel marker (position/cue/intro) server update.
    /// </summary>
    public class MarkerChangeArgs : ChannelArgsBase
    {
        /// <summary>
        ///     Constructs a channel marker server update.
        /// </summary>
        /// <param name="channelId">The ID of the channel whose marker is being moved.</param>
        /// <param name="marker">The marker being moved.</param>
        /// <param name="newValue">The new value of the marker.</param>
        public MarkerChangeArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId)
        {
            Marker = marker;
            NewValue = newValue;
        }

        /// <summary>
        ///     The specific marker being changed.
        /// </summary>
        public MarkerType Marker { get; }

        /// <summary>
        ///     The new value of the position marker being set.
        /// </summary>
        public uint NewValue { get; }
    }
}