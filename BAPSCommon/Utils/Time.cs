using System;

namespace BAPSClientCommon.Utils
{
    public static class Time
    {
        public static string MillisecondsToTimeString(int milliseconds)
        {
            return TimeSpanOfMilliseconds(milliseconds).ToString("hh\\:mm\\:ss");
        }

        private static TimeSpan TimeSpanOfMilliseconds(int milliseconds)
        {
            return TimeSpan.FromTicks(milliseconds * TimeSpan.TicksPerMillisecond);
        }
    }
}