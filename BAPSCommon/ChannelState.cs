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
                    return Command.PLAY;
                case ChannelState.Paused:
                    return Command.PAUSE;
                case ChannelState.Stopped:
                    return Command.STOP;
                default:
                    throw new ArgumentOutOfRangeException("this", pt, "Not a valid channel state");
            }
        }

        public static ChannelState AsChannelState(this Command c)
        {
            switch (c & Command.PLAYBACK_OPMASK)
            {
                case Command.PLAY:
                    return ChannelState.Playing;
                case Command.PAUSE:
                    return ChannelState.Paused;
                case Command.STOP:
                    return ChannelState.Stopped;
                default:
                    throw new ArgumentOutOfRangeException("this", c, "Command is not a valid channel state");
            }
        }
    }
}
