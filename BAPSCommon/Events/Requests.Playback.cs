using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Requests
    {
        /// <summary>
        ///     Payload for a channel marker (position/cue/intro) change request.
        /// </summary>
        public class MarkerEventArgs : Events.MarkerEventArgs
        {
            /// <summary>
            ///     Constructs a channel marker change request.
            /// </summary>
            /// <param name="channelId">The ID of the channel asking to move its marker.</param>
            /// <param name="marker">The marker being moved.</param>
            /// <param name="newValue">The new requested value of the marker.</param>
            public MarkerEventArgs(ushort channelId, MarkerType marker, uint newValue) : base(channelId, marker,
                newValue)
            {
            }
        }
    }
}