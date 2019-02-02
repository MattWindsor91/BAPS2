using BAPSClientCommon.Model;

namespace BAPSClientCommon.Events
{
    public static partial class Requests
    {
        /// <summary>
        ///     Event handler for channel marker change requests.
        /// </summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="e">The payload of this event.</param>
        public delegate void MarkerEventHandler(object sender, MarkerEventArgs e);

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