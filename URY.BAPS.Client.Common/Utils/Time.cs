using System;

namespace URY.BAPS.Client.Common.Utils
{
    public static class Time
    {
        private const string FormatWithoutHours = "mm\\:ss";

        /// <summary>
        ///     Converts a number of hours, minutes, and seconds to the
        ///     equivalent number of milliseconds.
        /// </summary>
        /// <param name="hours">The hours component.</param>
        /// <param name="minutes">The minutes component.</param>
        /// <param name="seconds">The seconds component.</param>
        /// <returns>The resulting number of milliseconds.</returns>
        public static uint BuildMilliseconds(ushort hours, ushort minutes, ushort seconds)
        {
            var totalMinutes = hours * 60u + minutes;
            var totalSeconds = totalMinutes * 60u + seconds;
            return totalSeconds * 1_000;
        }

        public static string MillisecondsToTimeString(uint milliseconds)
        {
            var span = TimeSpanOfMilliseconds(milliseconds);
            var minutesAndSeconds = span.ToString(FormatWithoutHours);

            // We can't use 'span.Hours' here, as it only accounts for hours between 0 and 23.
            var hours = (uint) Math.Floor(span.TotalHours);
            return 0 < hours ? $"{hours}:{minutesAndSeconds}" : minutesAndSeconds;
        }

        private static TimeSpan TimeSpanOfMilliseconds(uint milliseconds)
        {
            return TimeSpan.FromTicks(milliseconds * TimeSpan.TicksPerMillisecond);
        }
    }
}