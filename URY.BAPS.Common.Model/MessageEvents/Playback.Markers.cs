using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Common.Model.MessageEvents
{
    /// <summary>
    ///     Abstract base class of messages that involve channel markers.
    /// </summary>
    public class MarkerArgsBase : ChannelArgsBase
    {
        protected MarkerArgsBase(ushort channelId, MarkerType marker) : base(channelId)
        {
            Marker = marker;
        }

        /// <summary>
        ///     The specific marker being changed.
        /// </summary>
        public MarkerType Marker { get; }
    }

    /// <summary>
    ///     A message representing a client request for information about a
    ///     marker on a particular channel.
    /// </summary>
    public class MarkerGetArgs : MarkerArgsBase
    {
        public MarkerGetArgs(ushort channelId, MarkerType marker) : base(channelId, marker)
        {
        }
    }

    /// <summary>
    ///     Payload for a channel marker (position/cue/intro) change message.
    /// </summary>
    public class MarkerChangeArgs : MarkerArgsBase
    {
        /// <summary>
        ///     Constructs a channel marker server update.
        /// </summary>
        /// <param name="channelId">The ID of the channel whose marker is being moved.</param>
        /// <param name="marker">The marker being moved.</param>
        /// <param name="newValue">The new value of the marker.</param>
        /// <param name="status">Whether this is a request or a confirmation.</param>
        public MarkerChangeArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId, marker)
        {
            NewValue = newValue;
        }

        /// <summary>
        ///     The new value of the position marker being set.
        /// </summary>
        public uint NewValue { get; }
    }
}