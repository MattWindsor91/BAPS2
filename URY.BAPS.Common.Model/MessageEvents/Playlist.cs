using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Common.Model.MessageEvents
{

    /// <summary>
    ///     Payload for a track-list item delete server update.
    /// </summary>
    public class TrackDeleteArgs : TrackIndexArgsBase
    {
        public TrackDeleteArgs(TrackIndex index) : base(index)
        {
        }

        public override string ToString()
        {
            return $"TrackDelete ({Index})";
        }
    }

    /// <summary>
    ///     Payload for a track-list item move server update.
    /// </summary>
    public class TrackMoveArgs : TrackIndexArgsBase
    {
        public TrackMoveArgs(TrackIndex fromIndex, uint newPosition)
            : base(fromIndex)
        {
            NewPosition = newPosition;
        }

        public uint NewPosition { get; }

        public override string ToString()
        {
            return $"TrackMove ({Index} -> {NewPosition})";
        }
    }

    /// <summary>
    ///     Payload for a track-list item add server update.
    /// </summary>
    public class TrackAddArgs : TrackIndexArgsBase
    {
        public TrackAddArgs(TrackIndex index, ITrack item) : base(index)
        {
            Item = item;
        }

        public ITrack Item { get; }

        public override string ToString()
        {
            return $"TrackAdd: index {Index} is {Item}";
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