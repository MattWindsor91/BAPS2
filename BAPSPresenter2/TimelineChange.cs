using System;

namespace BAPSPresenter2
{
    /// <summary>
    /// Enumeration of types of timeline change.
    /// </summary>
    public enum TimelineChangeType
    {
        Start,
        Duration,
        Position
    }

    /// <summary>
    /// Argument bundle for a TimelineChangeEvent.
    /// </summary>
    public class TimelineChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the channel whose timeline needs updating.
        /// </summary>
        public ushort ChannelID { get; }

        /// <summary>
        /// The type of timeline reading that needs to change.
        /// </summary>
        public TimelineChangeType ChangeType { get; }

        /// <summary>
        /// The new value that the timeline should adopt.
        /// </summary>
        public int Value { get; }

        public TimelineChangeEventArgs(ushort channelID, TimelineChangeType type, int newValue) : base()
        {
            ChannelID = channelID;
            ChangeType = type;
            Value = newValue;
        }
    }

    /// <summary>
    /// Delegate type for timeline change requests.
    /// </summary>
    /// <param name="sender">The sender of the event (usually a channel).</param>
    /// <param name="e">The event details.</param>
    public delegate void TimelineChangeEventHandler(object sender, TimelineChangeEventArgs e);
}
