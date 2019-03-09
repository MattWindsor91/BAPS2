using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.Converters;
using Xunit;

namespace URY.BAPS.Client.Wpf.Tests.Converters
{
    /// <summary>
    ///     Tests that make sure that <see cref="MillisecondsToPositionStringConverter" />
    ///     produces the right strings for particular millisecond times.
    /// </summary>
    public class MillisecondsToPositionStringConverterTests
    {
        /// <summary>
        ///     The converter used in the tests.
        /// </summary>
        [NotNull] private readonly MillisecondsToPositionStringConverter _conv =
            new MillisecondsToPositionStringConverter();

        /// <summary>
        ///     Checks whether converting a uint to a position string works as expected.
        /// </summary>
        /// <param name="input">The input to test.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(0u, "00:00:00")]
        [InlineData(1_000u, "00:00:01")]
        [InlineData(59_999u, "00:00:59")]
        [InlineData(60_000u, "00:01:00")]
        [InlineData(3_599_999u, "00:59:59")]
        [InlineData(3_600_000u, "01:00:00")]
        [InlineData(3_723_000u, "01:02:03")]
        public void ConvertGivesExpectedResult(uint input, string expected)
        {
            var output = Assert.IsAssignableFrom<string>(_conv.Convert(input, typeof(string), null, null));
            Assert.Equal(expected, output);
        }
    }
}