using System;
using System.Globalization;
using System.Windows.Data;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Converts an unsigned integer containing a number of milliseconds to a
    /// string.
    /// </summary>
    public class MsecToPositionStringConverter : IValueConverter
    {
        public static string TimeToString(long hours, long minutes, long seconds, long centiseconds)
        {
            /** WORK NEEDED: fix me **/
            var mtemp = (minutes < 10) ? $"0{minutes}" : minutes.ToString();
            var stemp = (seconds < 10) ? $"0{minutes}" : seconds.ToString();
            return $"{hours}:{mtemp}:{stemp}";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is uint msecs)) return "??";
            if (msecs == 0) return "--:--:--";

            /** WORK NEEDED: lots **/
            var secs = msecs / 1000;

            var hours = Math.DivRem(secs, 3600, out _);
            var mins = Math.DivRem(secs, 60, out _) - (hours * 60);

            var lsecs = secs - ((mins * 60) + (hours * 3600));

            return TimeToString(hours, mins, lsecs, msecs % 1000 / 10);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0U; // FIXME
        }
    }
}
