using System;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    /// <summary>
    /// Enumeration of the various position markers that a channel's track-line contain.
    /// </summary>
    public enum MarkerType
    {
        Position,
        Cue,
        Intro
    }

    /// <summary>
    ///     Extension methods for <see cref="MarkerType"/>, and conversion therefrom and thereto.
    /// </summary>
    public static class MarkerTypeExtensions
    {
        public static Command AsCommand(this MarkerType pt)
        {
            switch (pt)
            {
                case MarkerType.Position:
                    return Command.Position;
                case MarkerType.Cue:
                    return Command.CuePosition;
                case MarkerType.Intro:
                    return Command.IntroPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid marker type");
            }
        }

        public static MarkerType AsMarkerType(this Command c)
        {
            switch (c & Command.PlaybackOpMask)
            {
                case Command.Position:
                    return MarkerType.Position;
                case Command.CuePosition:
                    return MarkerType.Cue;
                case Command.IntroPosition:
                    return MarkerType.Intro;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, "Command is not a valid marker type");
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
        public MarkerType ChangeType { get; }

        /// <summary>
        /// The new value that the timeline should adopt.
        /// </summary>
        public int Value { get; }

        public PositionRequestChangeEventArgs(ushort channelId, MarkerType type, int newValue)
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
