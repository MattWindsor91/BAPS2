using System;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon.Model
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
        public static Command AsCommand(this PlaybackState pt)
        {
            switch (pt)
            {
                case PlaybackState.Playing:
                    return Command.Playback | Command.Play;
                case PlaybackState.Paused:
                    return Command.Playback | Command.Pause;
                case PlaybackState.Stopped:
                    return Command.Playback | Command.Stop;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid channel state");
            }
        }

        public static PlaybackState AsPlaybackState(this Command c)
        {
            switch (c & Command.PlaybackOpMask)
            {
                case Command.Play:
                    return PlaybackState.Playing;
                case Command.Pause:
                    return PlaybackState.Paused;
                case Command.Stop:
                    return PlaybackState.Stopped;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, "Command is not a valid channel state");
            }
        }
    }
}