using System;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    /// <summary>
    ///     Enumeration of basic channel states.
    /// </summary>
    public enum ChannelState
    {
        Playing,
        Paused,
        Stopped
    }

    public static class ChannelStateExtensions
    {
        public static Command AsCommand(this ChannelState pt)
        {
            switch (pt)
            {
                case ChannelState.Playing:
                    return Command.Playback | Command.Play;
                case ChannelState.Paused:
                    return Command.Playback | Command.Pause;
                case ChannelState.Stopped:
                    return Command.Playback | Command.Stop;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pt), pt, "Not a valid channel state");
            }
        }

        public static ChannelState AsChannelState(this Command c)
        {
            switch (c & Command.PlaybackOpMask)
            {
                case Command.Play:
                    return ChannelState.Playing;
                case Command.Pause:
                    return ChannelState.Paused;
                case Command.Stop:
                    return ChannelState.Stopped;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, "Command is not a valid channel state");
            }
        }
    }
}