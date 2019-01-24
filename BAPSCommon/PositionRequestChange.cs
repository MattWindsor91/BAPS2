using System;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
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
        public static Command AsCommand(this PositionType pt)
        {
            switch (pt)
            {
                case PositionType.Position:
                    return Command.Position;
                case PositionType.Cue:
                    return Command.CuePosition;
                case PositionType.Intro:
                    return Command.IntroPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid position type");
            }
        }

        public static PositionType AsPositionType(this Command c)
        {
            switch (c & Command.PlaybackOpMask)
            {
                case Command.Position:
                    return PositionType.Position;
                case Command.CuePosition:
                    return PositionType.Cue;
                case Command.IntroPosition:
                    return PositionType.Intro;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, "Command is not a valid position type");
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// An event argument bundle used to tell the main presenter that a channel
    /// has received a position change request from the user.
    /// </summary>
    public class PositionRequestChangeEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the channel from where the request originates.
        /// </summary>
        public ushort ChannelId { get; }

        /// <summary>
        /// The type of requested position change.
        /// </summary>
        public PositionType ChangeType { get; }

        /// <summary>
        /// The new value that the timeline should adopt.
        /// </summary>
        public int Value { get; }

        public PositionRequestChangeEventArgs(ushort channelId, PositionType type, int newValue)
        {
            ChannelId = channelId;
            ChangeType = type;
            Value = newValue;
        }
    }

    /// <summary>
    /// Delegate type for position change requests.
    /// </summary>
    /// <param name="sender">The sender of the event (usually a channel).</param>
    /// <param name="e">The event details.</param>
    public delegate void PositionRequestChangeEventHandler(object sender, PositionRequestChangeEventArgs e);
}
