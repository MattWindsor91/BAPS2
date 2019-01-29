using System;
using System.Globalization;
using System.Windows.Data;
using BAPSClientCommon.Model;
using FontAwesome.WPF;

namespace BAPSPresenterNG.Converters
{
    /// <summary>
    /// Converts the type of a track-list entry to a Font Awesome icon.
    /// </summary>
    public class EntryTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ITrack entry)) return FontAwesomeIcon.Question;

            if (entry.IsTextItem) return FontAwesomeIcon.CommentOutline;
            if (entry.IsAudioItem && entry.IsFromLibrary) return FontAwesomeIcon.Music;
            if (entry.IsAudioItem) return FontAwesomeIcon.FileAudioOutline;
            return FontAwesomeIcon.Question;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NullTrack();
        }
    }
}
