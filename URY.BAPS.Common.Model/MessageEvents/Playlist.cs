using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Payload for a track-list item delete server update.
    /// </summary>
    public class TrackDeleteArgs : TrackIndexArgsBase
    {
        public TrackDeleteArgs(ushort channelId, uint index) : base(channelId, index)
        {
        }

        public override string ToString()
        {
            return $"TrackDelete: channel {ChannelId} index {Index}";
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

        public override string ToString()
        {
            return $"TrackMove: channel {ChannelId} index {Index} -> {NewIndex}";
        }
    }

    /// <summary>
    ///     Payload for a track-list item add server update.
    /// </summary>
    public class TrackAddArgs : TrackIndexArgsBase
    {
        public TrackAddArgs(ushort channelId, uint index, ITrack item)
            : base(channelId, index)
        {
            Item = item;
        }

        public ITrack Item { get; }

        public override string ToString()
        {
            return $"TrackAdd: channel {ChannelId} index {Index} is {Item}";
        }
    }

    /// <summary>
    ///     Payload for a channel track-list reset server update.
    /// </summary>
    public class PlaylistResetArgs : ChannelArgsBase
    {
        public PlaylistResetArgs(ushort channelId) : base(channelId)
        {
        }

        public override string ToString()
        {
            return $"PlaylistReset: channel {ChannelId}";
        }
    }
}