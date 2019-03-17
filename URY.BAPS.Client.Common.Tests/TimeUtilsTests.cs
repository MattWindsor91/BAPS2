using URY.BAPS.Client.Common.Utils;
using Xunit;

namespace URY.BAPS.Client.Common.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Time" /> static class.
    /// </summary>
    public class TimeUtilsTests
    {
        public static TheoryData<ushort, ushort, ushort, string> MillisecondsToTimeStringData =>
            new TheoryData<ushort, ushort, ushort, string>
            {
                {1, 2, 3, "1:02:03"},
                {21, 42, 53, "21:42:53"},
                {0, 0, 0, "00:00"},
                {100, 0, 0, "100:00:00"}
            };

        [Theory, MemberData(nameof(MillisecondsToTimeStringData))]
        public void TestMillisecondsToTimeString(ushort hours, ushort minutes, ushort seconds, string expected)
        {
            var time = Time.BuildMilliseconds(hours, minutes, seconds);
            Assert.Equal(expected, Time.MillisecondsToTimeString(time));
        }
    }
}