using System;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon.Model
{
    /// <summary>
    ///     Enumeration of the various position markers that a channel's track-line contain.
    /// </summary>
    public enum MarkerType
    {
        Position,
        Cue,
        Intro
    }

    /// <summary>
    ///     Extension methods for <see cref="MarkerType" />, and conversion therefrom and thereto.
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
}