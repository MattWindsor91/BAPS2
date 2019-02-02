using System;
using System.Globalization;
using System.Windows.Data;

namespace BAPSPresenterNG.Converters
{
    public class PositionScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value[0] is double scale)) return 0.0;
            if (!(value[1] is double totalWidth)) return 0.0;
            return ConvertTyped(scale, totalWidth);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }

        public double ConvertTyped(double scale, double totalWidth)
        {
            var markerWidth = totalWidth * scale;
            if (totalWidth < markerWidth) return totalWidth;
            if (markerWidth < 0) return 0;
            return markerWidth;
        }
    }
}