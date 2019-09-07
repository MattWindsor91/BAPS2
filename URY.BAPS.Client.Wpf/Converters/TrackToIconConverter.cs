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
            switch (value)
            {
                case ITrack entry when entry.IsError: return EFontAwesomeIcon.Solid_ExclamationTriangle;
                case ITrack entry when entry.IsLoading: return EFontAwesomeIcon.Solid_EllipsisH;
                case ITrack entry when entry.IsTextItem: return EFontAwesomeIcon.Regular_Comment;
                case ITrack entry when entry.IsAudioItem && entry.IsFromLibrary: return EFontAwesomeIcon.Solid_Music;
                case ITrack entry when entry.IsAudioItem: return EFontAwesomeIcon.Regular_FileAudio;
                default: return EFontAwesomeIcon.Solid_Question;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new NullTrack();
        }
    }
}