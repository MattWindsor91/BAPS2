using System;

namespace BAPSCommon
{
    /// <summary>
    /// Enumeration of basic channel states.
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
                    return Command.Play;
                case ChannelState.Paused:
                    return Command.Pause;
                case ChannelState.Stopped:
                    return Command.Stop;
                default:
                    throw new ArgumentOutOfRangeException("this", pt, "Not a valid channel state");
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
                    throw new ArgumentOutOfRangeException("this", c, "Command is not a valid channel state");
            }
        }
    }
}
