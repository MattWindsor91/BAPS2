using System;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Common.Protocol.V2.Commands
{
    // The numbers assigned to each of these enum values are significant:
    // when shifted left by the appropriate member of CommandShifts,
    // they form the BapsNet flags.
    // As such, _do not_ change them without good reason.

    /// <summary>
    ///     Enumeration of playback operations.
    /// </summary>
    public enum PlaybackOp : byte
    {
        Play = 0,
        Stop = 1,
        Pause = 2,
        Position = 3,
        Volume = 4,
        Load = 5,
        CuePosition = 6,
        IntroPosition = 7
    }

    public static class PlaybackOpExtensions
    {
        public static PlaybackOp AsPlaybackOp(this PlaybackState pt)
        {
            return pt switch
                {
                PlaybackState.Playing => PlaybackOp.Play,
                PlaybackState.Paused => PlaybackOp.Pause,
                PlaybackState.Stopped => PlaybackOp.Stop,
                _ => throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid channel state")
                };
        }

        public static PlaybackState AsPlaybackState(this PlaybackOp op)
        {
            return op switch {
                PlaybackOp.Play => PlaybackState.Playing,
                PlaybackOp.Pause => PlaybackState.Paused,
                PlaybackOp.Stop => PlaybackState.Stopped,
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, "Command is not a valid channel state")
                };
        }

        public static CommandWord AsCommandWord(this PlaybackState pt)
        {
            return pt.AsPlaybackOp().AsCommandWord();
        }

        public static PlaybackState AsPlaybackState(this CommandWord c)
        {
            return c.PlaybackOp().AsPlaybackState();
        }

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