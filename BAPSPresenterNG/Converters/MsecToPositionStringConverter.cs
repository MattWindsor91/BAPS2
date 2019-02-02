using System;
using System.Globalization;
using System.Windows.Data;
using BAPSClientCommon.Utils;

namespace BAPSPresenterNG.Converters
{
    /// <summary>
    ///     Converts an unsigned integer containing a number of milliseconds to a
    ///     string.
    /// </summary>
    public class MsecToPositionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is uint msecs)) return "??";
            return Time.MillisecondsToTimeString((int) msecs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0U; // FIXME
        }
    }
}