using URY.BAPS.Common.Model.MessageEvents;
using Xunit;

namespace URY.BAPS.Common.Model.Tests.MessageEvents
{
    /// <summary>
    ///     Tests for <see cref="CountArgs"/>.
    /// </summary>
    public class CountArgsTests
    {
        /// <summary>
        ///     Pairs of count message and expected string representation.
        /// </summary>
        public static TheoryData<CountArgs, string> CountAndStringData => new TheoryData<CountArgs, string>
        {
            { new CountArgs(CountType.Show, 0, 0), "Count: 0 item(s) of type Show incoming (index 0)" },
            { new CountArgs(CountType.User, 1, 0), "Count: 1 item(s) of type User incoming (index 0)" },
            { new CountArgs(CountType.ConfigChoice, 10, 42), "Count: 10 item(s) of type ConfigChoice incoming (index 42)" }
        };

        [Theory]
        [MemberData(nameof(CountAndStringData))]
        public void TestToString(CountArgs count, string expected) {
            Assert.Equal(expected, count.ToString());
        }
    }
}