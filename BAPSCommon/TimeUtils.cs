using System;

namespace BAPSCommon
{
    public static class TimeUtils
    {
        public static string MillisecondsToTimeString(int msecs) =>
            TimeSpanOfMilliseconds(msecs).ToString("hh\\:mm\\:ss");

        public static TimeSpan TimeSpanOfMilliseconds(int msecs) =>
            TimeSpan.FromTicks(msecs * TimeSpan.TicksPerMillisecond);
    }
}
