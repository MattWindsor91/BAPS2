using System;

namespace BAPSClientCommon.Utils
{
    public static class Time
    {
        public static string MillisecondsToTimeString(int milliseconds) =>
            TimeSpanOfMilliseconds(milliseconds).ToString("hh\\:mm\\:ss");

        public static TimeSpan TimeSpanOfMilliseconds(int milliseconds) =>
            TimeSpan.FromTicks(milliseconds * TimeSpan.TicksPerMillisecond);
    }
}
