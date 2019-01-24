using BAPSClientCommon;
using NUnit.Framework;

namespace BAPSClientCommonTests
{
    /// <summary>
    /// Tests for the <see cref="TimeUtils"/> static class.
    /// </summary>
    class TimeUtilsTests
    {
        private int BuildMsec(ushort hours, ushort minutes, ushort seconds)
        {
            var totalMinutes = (hours * 60) + minutes;
            var totalSeconds = (totalMinutes * 60) + seconds;
            return totalSeconds * 1_000;
        }

        [Test]
        public void TestMillisecondsToTimeString()
        {
            var time1 = BuildMsec(1, 2, 3);
            Assert.That(TimeUtils.MillisecondsToTimeString(time1), Is.EqualTo("01:02:03"));

            var time2 = BuildMsec(21, 42, 53);
            Assert.That(TimeUtils.MillisecondsToTimeString(time2), Is.EqualTo("21:42:53"));

            var time3 = BuildMsec(0, 0, 0);
            Assert.That(TimeUtils.MillisecondsToTimeString(time3), Is.EqualTo("00:00:00"));
        }
    }
}
