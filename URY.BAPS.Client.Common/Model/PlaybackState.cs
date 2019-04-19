using System;
using URY.BAPS.Protocol.V2.Commands;

namespace URY.BAPS.Client.Common.Model
{
    /// <summary>
    ///     Enumeration of basic channel states.
    /// </summary>
    public enum PlaybackState
    {
        Playing,
        Paused,
        Stopped
    }

    public static class PlaybackStateExtensions
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
    }
}