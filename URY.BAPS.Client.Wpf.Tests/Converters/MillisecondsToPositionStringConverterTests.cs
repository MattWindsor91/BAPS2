using URY.BAPS.Client.Common.Utils;
using URY.BAPS.Client.Wpf.Converters;

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
        ///     Data for <see cref="ConvertGivesExpectedResult" />.
        /// </summary>
        public static TheoryData<ushort, ushort, ushort, string> ConvertGivesExpectedResultData =>
            new TheoryData<ushort, ushort, ushort, string>
            {
                {0, 0, 0, "   00:00"},
                {0, 0, 1, "   00:01"},
                {0, 0, 59, "   00:59"},
                {0, 1, 0, "   01:00"},
                {0, 59, 59, "   59:59"},
                {1, 0, 0, " 1:00:00"},
                {1, 2, 3, " 1:02:03"},
                {20, 0, 0, "20:00:00"},
                {100, 0, 0, MillisecondsToPositionStringConverter.Indeterminate}
            };

        /// <summary>
        ///     Checks whether converting a uint to a position string works as expected.
        /// </summary>
        /// <param name="h">The number of hours to input.</param>
        /// <param name="m">The number of minutes to input.</param>
        /// <param name="s">The number of seconds to input.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [MemberData(nameof(ConvertGivesExpectedResultData))]
        public void ConvertGivesExpectedResult(int h, int m, int s, string expected)
        {
            var input = Time.BuildMilliseconds((ushort) h, (ushort) m, (ushort) s);
            var output = _conv.Convert(input, typeof(string), null, null);
            var outputStr = Assert.IsAssignableFrom<string>(output);
            Assert.Equal(expected, outputStr);
        }

        /// <summary>
        ///     Checks whether converting an object of invalid type gives the indeterminate string.
        /// </summary>
        /// <param name="input">The (wrong-type) object to test.</param>
        [Theory]
        [ClassData(typeof(ConvertHandlesInvalidTypeCorrectlyData))]
        public void ConvertHandlesInvalidTypeCorrectly(object input)
        {
            var output = _conv.Convert(input, typeof(string), null, null);
            var outputStr = Assert.IsAssignableFrom<string>(output);
            Assert.Equal(MillisecondsToPositionStringConverter.Indeterminate, outputStr);
        }

        /// <summary>
        ///     Data for <see cref="ConvertHandlesInvalidTypeCorrectly" />.
        /// </summary>
        public class ConvertHandlesInvalidTypeCorrectlyData : TheoryData<object>
        {
            public ConvertHandlesInvalidTypeCorrectlyData()
            {
                Add("not an integer!");
                Add(3.14159);
                Add(new[] {2, 4, 6, 8});
            }
        }
    }
}