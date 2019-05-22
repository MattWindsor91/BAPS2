using System;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Common.Protocol.V2.Tests.Utils
{
    /// <summary>
    ///     Test helper extensions for tracks.
    /// </summary>
    public static class TrackExtensions
    {
        
        /// <summary>
        ///     Creates a compact string summary of a track, useful for testing.
        /// </summary>
        /// <param name="track">The track to summarise.</param>
        /// <returns>
        ///     A string containing several pipe-delimited sections:
        ///     first, a letter each (capital if true, lowercase if false)
        ///     for the error (E), loading (L), audio-item (A), library-item (I), and text-item (T)
        ///     flags; then the description, duration, and text field of the track.
        /// </returns>
        [Pure]
        public static string Summarise(this ITrack track)
        {
            return string.Join('|',
                track.IsError ? 'E' : 'e',
                track.IsLoading ? 'L' : 'l',
                track.IsAudioItem ? 'A' : 'a',
                track.IsFromLibrary ? 'I' : 'i',
                track.IsTextItem ? 'T' : 't',
                track.Description,
                track.Duration,
                track.Text
             );
        }
    }
}