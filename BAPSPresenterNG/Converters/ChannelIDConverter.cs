using System;
using System.Globalization;
using System.Windows.Data;

namespace BAPSPresenterNG.Converters
{
    /// <summary>
    /// A value converter that maps from internal channel IDs to human-friendly channel numbers.
    /// </summary>
    public class ChannelIDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort channelID)
            {
                return (channelID + 1).ToString();
            }
            return "??";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort result = 0;

            if (value is string channelNumberString)
            {
                ushort.TryParse(channelNumberString, out result);
                result--;
            }

            return result;
        }
    }
}
