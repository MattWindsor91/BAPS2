using System;
using System.Globalization;
using System.Windows.Data;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts an unsigned integer containing a number of milliseconds to a
    ///     string.
    /// </summary>
    public class MillisecondsToPositionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is uint ms ? Time.MillisecondsToTimeString((int) ms) : "??";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0U; // FIXME
        }
    }
}