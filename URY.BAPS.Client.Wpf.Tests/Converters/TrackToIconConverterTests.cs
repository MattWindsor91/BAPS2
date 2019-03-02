using System.Globalization;
using FontAwesome.WPF;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Wpf.Converters;
using Xunit;

namespace URY.BAPS.Client.Wpf.Tests.Converters
{
    /// <summary>
    ///     Tests that make sure that <see cref="TrackToIconConverter" />
    ///     produces the right icons for each track type.
    /// </summary>
    public class TrackToIconConverterTests
    {
        /// <summary>
        ///     The converter used in the tests.
        /// </summary>
        [NotNull] private readonly TrackToIconConverter _conv = new TrackToIconConverter();

        /// <summary>
        ///     Runs the converter on the given track.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        /// <returns>The resulting icon (or null if the converter didn't produce a valid icon object).</returns>
        private FontAwesomeIcon? Run(ITrack track)
        {
            var obj = _conv.Convert(
                track,
                typeof(FontAwesomeIcon),
                null,
                CultureInfo.CurrentCulture);
            return obj as FontAwesomeIcon?;
        }

        /// <summary>
        ///     Asserts that the error track converts to a warning icon.
        /// </summary>
        [Fact]
        public void Test_Convert_ErrorTrack_WarningIcon()
        {
            Assert.Equal(FontAwesomeIcon.Warning, Run(new ErrorTrack()));
        }

        /// <summary>
        ///     Asserts that a file track converts to an audio-file icon.
        /// </summary>
        [Fact]
        public void Test_Convert_FileTrack_FileIcon()
        {
            Assert.Equal(FontAwesomeIcon.FileAudioOutline, Run(new FileTrack("foo", 3600)));
        }

        /// <summary>
        ///     Asserts that a library track converts to a music icon.
        /// </summary>
        [Fact]
        public void Test_Convert_LibraryTrack_MusicIcon()
        {
            Assert.Equal(FontAwesomeIcon.Music, Run(new LibraryTrack("foo", 3600)));
        }

        /// <summary>
        ///     Asserts that the loading track converts to an ellipsis icon.
        /// </summary>
        [Fact]
        public void Test_Convert_LoadingTrack_EllipsisIcon()
        {
            Assert.Equal(FontAwesomeIcon.EllipsisH, Run(new LoadingTrack()));
        }

        /// <summary>
        ///     Asserts that a null track converts to a question-mark icon.
        /// </summary>
        [Fact]
        public void Test_Convert_NullTrack_QuestionIcon()
        {
            Assert.Equal(FontAwesomeIcon.Question, Run(new NullTrack()));
        }

        /// <summary>
        ///     Asserts that a text track converts to a speech icon.
        /// </summary>
        [Fact]
        public void Test_Convert_TextTrack_SpeechIcon()
        {
            Assert.Equal(FontAwesomeIcon.CommentOutline, Run(new TextTrack("foo", "bar")));
        }

        /// <summary>
        ///     Asserts that an icon converts back to a null track.
        /// </summary>
        [Fact]
        public void Test_ConvertBack_NullTrack()
        {
            var trackObject = _conv.ConvertBack(FontAwesomeIcon.FileAudioOutline, typeof(Track), null,
                CultureInfo.CurrentCulture);
            Assert.NotNull(trackObject);
            Assert.IsAssignableFrom<ITrack>(trackObject);
            var track = (ITrack) trackObject;
            Assert.False(track.IsTextItem);
            Assert.False(track.IsError);
            Assert.False(track.IsLoading);
            Assert.False(track.IsAudioItem);
            Assert.False(track.IsFromLibrary);
        }
    }
}