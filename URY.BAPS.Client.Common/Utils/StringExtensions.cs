using System;

namespace URY.BAPS.Client.Common.Utils
{
    /// <summary>
    ///     String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     The default maximum length of <see cref="Summary" />.
        /// </summary>
        private const ushort DefaultSummaryLength = 32;

        /// <summary>
        ///     The ellipsis that <see cref="Summary" /> adds to over-long strings.
        /// </summary>
        private const string Ellipsis = "\u2026";

        /// <summary>
        ///     Trims down a string to fit a particular length, adding an ellipsis.
        /// </summary>
        /// <param name="longString">The string to trim.</param>
        /// <param name="maxLength">An optional length to trim to; the default is <see cref="DefaultSummaryLength" />.</param>
        /// <returns></returns>
        public static string Summary(this string longString, ushort maxLength = DefaultSummaryLength)
        {
            if (maxLength == 0) throw new ArgumentOutOfRangeException(nameof(maxLength), "cannot be zero");

            if (longString.Length <= maxLength) return longString;
            var trimmed = longString.Substring(0, maxLength - 1);
            return trimmed + Ellipsis;
        }
    }
}