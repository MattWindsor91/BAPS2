using NUnit.Framework;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Time" /> static class.
    /// </summary>
    internal class TimeUtilsTests
    {
        private static int BuildMilliseconds(ushort hours, ushort minutes, ushort seconds)
        {
            var totalMinutes = hours * 60 + minutes;
            var totalSeconds = totalMinutes * 60 + seconds;
            return totalSeconds * 1_000;
        }

        [Test]
        public void TestMillisecondsToTimeString()
        {
            var time1 = BuildMilliseconds(1, 2, 3);
            Assert.That(Time.MillisecondsToTimeString(time1), Is.EqualTo("01:02:03"));

            var time2 = BuildMilliseconds(21, 42, 53);
            Assert.That(Time.MillisecondsToTimeString(time2), Is.EqualTo("21:42:53"));

            var time3 = BuildMilliseconds(0, 0, 0);
            Assert.That(Time.MillisecondsToTimeString(time3), Is.EqualTo("00:00:00"));
        }
    }
}