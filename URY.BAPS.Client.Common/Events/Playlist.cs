using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Events
{
    /// <summary>
    ///     Payload for a track-list item delete server update.
    /// </summary>
    public class TrackDeleteEventArgs : TrackIndexEventArgsBase
    {
        public TrackDeleteEventArgs(ushort channelId, uint index) : base(channelId, index)
        {
        }
    }

    /// <summary>
    ///     Payload for a track-list item move server update.
    /// </summary>
    public class TrackMoveEventArgs : TrackIndexEventArgsBase
    {
        public TrackMoveEventArgs(ushort channelId, uint fromIndex, uint toIndex)
            : base(channelId, fromIndex)
        {
            NewIndex = toIndex;
        }

        public uint NewIndex { get; }
    }

    /// <summary>
    ///     Payload for a track-list item add server update.
    /// </summary>
    public class TrackAddEventArgs : TrackIndexEventArgsBase
    {
        public TrackAddEventArgs(ushort channelId, uint index, Track item)
            : base(channelId, index)
        {
            Item = item;
        }

        public Track Item { get; }
    }

    /// <summary>
    ///     Payload for a channel track-list reset server update.
    /// </summary>
    public class PlaylistResetEventArgs : ChannelEventArgsBase
    {
        public PlaylistResetEventArgs(ushort channelId) : base(channelId)
        {
        }
    }
}