using System;

namespace BAPSPresenter2
{
    /// <summary>
    /// Enumeration of the various positions that a channel's track-line represents.
    /// </summary>
    public enum PositionType
    {
        Position,
        Cue,
        Intro
    }

    public static class PositionTypeExtensions
    {
        internal static Command AsCommand(this PositionType pt)
        {
            switch (pt)
            {
                case PositionType.Position:
                    return Command.POSITION;
                case PositionType.Cue:
                    return Command.CUEPOSITION;
                case PositionType.Intro:
                    return Command.INTROPOSITION;
                default:
                    throw new ArgumentOutOfRangeException("this", pt, "Not a valid position type");
            }
        }
    }

    /// <summary>
    /// An event argument bundle used to tell the main presenter that a channel
    /// has received a position change request from the user.
    /// </summary>
    public class PositionRequestChange : EventArgs
    {
        /// <summary>
        /// The ID of the channel from where the request originates.
        /// </summary>
        public ushort ChannelID { get; }

        /// <summary>
        /// The type of requested position change.
        /// </summary>
        public PositionType ChangeType { get; }

        /// <summary>
        /// The new value that the timeline should adopt.
        /// </summary>
        public int Value { get; }

        public PositionRequestChange(ushort channelID, PositionType type, int newValue) : base()
        {
            ChannelID = channelID;
            ChangeType = type;
            Value = newValue;
        }
    }

    /// <summary>
    /// Delegate type for position change requests.
    /// </summary>
    /// <param name="sender">The sender of the event (usually a channel).</param>
    /// <param name="e">The event details.</param>
    public delegate void PositionRequestChangeEventHandler(object sender, PositionRequestChange e);
}
