using System;
using System.Globalization;
using System.Windows.Data;
using FontAwesome.WPF;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts the type of a track-list entry to a Font Awesome icon.
    /// </summary>
    public class TrackToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ITrack entry when entry.IsError: return FontAwesomeIcon.Warning;
                case ITrack entry when entry.IsLoading: return FontAwesomeIcon.EllipsisH;
                case ITrack entry when entry.IsTextItem: return FontAwesomeIcon.CommentOutline;
                case ITrack entry when entry.IsAudioItem && entry.IsFromLibrary: return FontAwesomeIcon.Music;
                case ITrack entry when entry.IsAudioItem: return FontAwesomeIcon.FileAudioOutline;
                default: return FontAwesomeIcon.Question;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NullTrack();
        }
    }
}