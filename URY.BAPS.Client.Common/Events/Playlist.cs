using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.Events
{
    /// <summary>
    ///     Payload for a track-list item delete server update.
    /// </summary>
    public class TrackDeleteArgs : TrackIndexArgsBase
    {
        public TrackDeleteArgs(ushort channelId, uint index) : base(channelId, index)
        {
        }
    }

    /// <summary>
    ///     Payload for a track-list item move server update.
    /// </summary>
    public class TrackMoveArgs : TrackIndexArgsBase
    {
        public TrackMoveArgs(ushort channelId, uint fromIndex, uint toIndex)
            : base(channelId, fromIndex)
        {
            NewIndex = toIndex;
        }

        public uint NewIndex { get; }
    }

    /// <summary>
    ///     Payload for a track-list item add server update.
    /// </summary>
    public class TrackAddArgs : TrackIndexArgsBase
    {
        public TrackAddArgs(ushort channelId, uint index, Track item)
            : base(channelId, index)
        {
            Item = item;
        }

        public Track Item { get; }
    }

    /// <summary>
    ///     Payload for a channel track-list reset server update.
    /// </summary>
    public class PlaylistResetArgs : ChannelArgsBase
    {
        public PlaylistResetArgs(ushort channelId) : base(channelId)
        {
        }
    }
}