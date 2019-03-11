using NUnit.Framework;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Time" /> static class.
    /// </summary>
    internal class TimeUtilsTests
    {
        [Test]
        public void TestMillisecondsToTimeString()
        {
            var time1 = Time.BuildMilliseconds(1, 2, 3);
            Assert.That(Time.MillisecondsToTimeString(time1), Is.EqualTo("1:02:03"));

            var time2 = Time.BuildMilliseconds(21, 42, 53);
            Assert.That(Time.MillisecondsToTimeString(time2), Is.EqualTo("21:42:53"));

            var time3 = Time.BuildMilliseconds(0, 0, 0);
            Assert.That(Time.MillisecondsToTimeString(time3), Is.EqualTo("00:00"));

            var time4 = Time.BuildMilliseconds(100, 0, 0);
            Assert.That(Time.MillisecondsToTimeString(time4), Is.EqualTo("100:00:00"));
        }
    }
}