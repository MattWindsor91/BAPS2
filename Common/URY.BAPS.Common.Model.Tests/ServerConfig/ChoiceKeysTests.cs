using URY.BAPS.Common.Model.ServerConfig;
using Xunit;

namespace URY.BAPS.Common.Model.Tests.ServerConfig
{
    /// <summary>
    ///     Tests various <see cref="ChoiceKeys" /> extensions and related code.
    /// </summary>
    public class ChoiceKeysTests
    {
        /// <summary>
        ///     Tests that <code>false</code> maps to <see cref="ChoiceKeys.No" />.
        /// </summary>
        [Fact]
        public void TestBooleanToChoice_FalseIsNo()
        {
            Assert.Equal(ChoiceKeys.No, ChoiceKeys.BooleanToChoice(false));
        }

        /// <summary>
        ///     Tests that <code>true</code> maps to <see cref="ChoiceKeys.Yes" />.
        /// </summary>
        [Fact]
        public void TestBooleanToChoice_TrueIsYes()
        {
            Assert.Equal(ChoiceKeys.Yes, ChoiceKeys.BooleanToChoice(true));
        }

        public static TheoryData<bool> FallbackData => new TheoryData<bool> { true, false };

        public static TheoryData<string, bool> InvalidChoiceData {
            get {
                var data = new TheoryData<string, bool>();
                foreach (var choice in new[]{"", ChoiceKeys.RepeatAll, "いいえ"})
                {
                    foreach (var fallback in new[] { true, false }) data.Add(choice, fallback);
                }
                return data;
            }
        }

        /// <summary>
        ///     Tests that various invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with invalid choice keys
        ///     returns the fallback.
        /// </summary>
        /// <param name="input">The input to test against.</param>
        /// <param name="fallback">The fallback to test against.</param>
        [Theory, MemberData(nameof(InvalidChoiceData))]
        public void TestChoiceToBoolean_AnythingElseIsFallback(string input, bool fallback)
        {
            Assert.Equal(fallback, ChoiceKeys.ChoiceToBoolean(input, fallback));
        }

        /// <summary>
        ///     Tests that invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with <see cref="ChoiceKeys.No" />
        ///     returns <code>false</code>.
        /// </summary>
        /// <param name="fallback">The fallback to test against.</param>
        [Theory, MemberData(nameof(FallbackData))]
        public void TestChoiceToBoolean_NoIsFalse(bool fallback)
        {
            Assert.False(ChoiceKeys.ChoiceToBoolean(ChoiceKeys.No, fallback));
        }

        /// <summary>
        ///     Tests that invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with <see cref="ChoiceKeys.Yes" />
        ///     returns <code>true</code>.
        /// </summary>
        /// <param name="fallback">The fallback to test against.</param>
        [Theory, MemberData(nameof(FallbackData))]
        public void TestChoiceToBoolean_YesIsTrue(bool fallback)
        {
            Assert.True(ChoiceKeys.ChoiceToBoolean(ChoiceKeys.Yes, fallback));
        }
    }
}