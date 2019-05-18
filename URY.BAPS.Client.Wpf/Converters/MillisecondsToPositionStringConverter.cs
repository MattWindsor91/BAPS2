using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts an unsigned integer containing a number of milliseconds to a
    ///     string.  The string may be left-padded with spaces.
    /// </summary>
    public class MillisecondsToPositionStringConverter : IValueConverter
    {
        /// <summary>
        ///     Expected length of the position string.
        /// </summary>
        private const int TargetLength = 8; // XX:XX:XX

        /// <summary>
        ///     String returned when the conversion fails or overflows.
        /// </summary>
        public const string Indeterminate = "88:88:88";

        private string ConvertMilliseconds(uint ms)
        {
            var result = Time.MillisecondsToTimeString(ms).PadLeft(TargetLength);
            return TargetLength < result.Length ? Indeterminate : result;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is uint ms ? ConvertMilliseconds(ms) : Indeterminate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0U; // FIXME
        }
    }
}