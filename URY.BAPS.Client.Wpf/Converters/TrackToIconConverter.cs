using System;
using System.Globalization;
using System.Windows.Data;
using FontAwesome5;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.Wpf.Converters
{
    /// <summary>
    ///     Converts the type of a track-list entry to a Font Awesome icon.
    /// </summary>
    public class TrackToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                ITrack entry when entry.IsError => EFontAwesomeIcon.Solid_ExclamationTriangle,
                ITrack entry when entry.IsLoading => EFontAwesomeIcon.Solid_EllipsisH,
                ITrack entry when entry.IsTextItem => EFontAwesomeIcon.Regular_Comment,
                ITrack entry when entry.IsAudioItem && entry.IsFromLibrary => EFontAwesomeIcon.Solid_Music,
                ITrack entry when entry.IsAudioItem => EFontAwesomeIcon.Regular_FileAudio,
                _ => EFontAwesomeIcon.Solid_Question
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new NullTrack();
        }
    }
}