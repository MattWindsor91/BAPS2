using System;

namespace BAPSCommon
{
    public static class TimeUtils
    {
        public static string MillisecondsToTimeString(int milliseconds) =>
            TimeSpanOfMilliseconds(milliseconds).ToString("hh\\:mm\\:ss");

        public static TimeSpan TimeSpanOfMilliseconds(int milliseconds) =>
            TimeSpan.FromTicks(milliseconds * TimeSpan.TicksPerMillisecond);
    }
}
