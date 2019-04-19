using System;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common.Model
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
        public static PlaybackOp AsPlaybackOp(this MarkerType pt)
        {
            return pt switch
                {
                MarkerType.Position => PlaybackOp.Position,
                MarkerType.Cue => PlaybackOp.CuePosition,
                MarkerType.Intro => PlaybackOp.IntroPosition,
                _ => throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid marker type")
                };
        }

        public static CommandWord AsCommandWord(this MarkerType pt)
        {
            return pt.AsPlaybackOp().AsCommandWord();
        }

        public static MarkerType AsMarkerType(this PlaybackOp op)
        {
            return op switch
                {
                PlaybackOp.Position => MarkerType.Position,
                PlaybackOp.CuePosition => MarkerType.Cue,
                PlaybackOp.IntroPosition => MarkerType.Intro,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, "Op is not a valid marker type")
                };
        }

        public static MarkerType AsMarkerType(this CommandWord c)
        {
            return c.PlaybackOp().AsMarkerType();
        }
    }
}