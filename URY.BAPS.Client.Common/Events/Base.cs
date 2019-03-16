namespace URY.BAPS.Client.Common.Events
{
    /// <summary>
    ///     Abstract base class of event payloads that reference
    ///     indices in channel track lists.
    /// </summary>
    public abstract class TrackIndexEventArgsBase : ChannelEventArgsBase
    {
        protected TrackIndexEventArgsBase(ushort channelId, uint index) : base(channelId)
        {
            Index = index;
        }

        /// <summary>
        ///     The track-list index being mentioned in the event.
        /// </summary>
        public uint Index { get; }
    }

    public struct ErrorEventArgs
    {
        public ErrorType Type;
        public byte Code;
        public string Description;
    }

    public abstract class ChannelEventArgsBase
    {
        protected ChannelEventArgsBase(ushort channelId)
        {
            ChannelId = channelId;
        }

        public ushort ChannelId { get; }
    }

    /// <summary>
    ///     Enumeration of different errors a BAPS server can send.
    /// </summary>
    public enum ErrorType
    {
        Library,
        BapsDb,
        Config
    }
}