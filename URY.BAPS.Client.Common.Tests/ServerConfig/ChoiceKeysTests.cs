using NUnit.Framework;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Tests.ServerConfig
{
    /// <summary>
    ///     Tests various <see cref="ChoiceKeys" /> extensions and related code.
    /// </summary>
    [TestFixture]
    public class ChoiceKeysTests
    {
        /// <summary>
        ///     Tests that <code>false</code> maps to <see cref="ChoiceKeys.No" />.
        /// </summary>
        [Test]
        public void TestBooleanToChoice_FalseIsNo()
        {
            Assert.That(ChoiceKeys.BooleanToChoice(false), Is.EqualTo(ChoiceKeys.No));
        }

        /// <summary>
        ///     Tests that <code>true</code> maps to <see cref="ChoiceKeys.Yes" />.
        /// </summary>
        [Test]
        public void TestBooleanToChoice_TrueIsYes()
        {
            Assert.That(ChoiceKeys.BooleanToChoice(true), Is.EqualTo(ChoiceKeys.Yes));
        }

        /// <summary>
        ///     Tests that various invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with invalid choice keys
        ///     returns the fallback.
        /// </summary>
        /// <param name="input">The input to test against.</param>
        /// <param name="fallback">The fallback to test against.</param>
        [Test]
        [Combinatorial]
        public void TestChoiceToBoolean_AnythingElseIsFallback([Values("", ChoiceKeys.RepeatAll, "いいえ")]
            string input, [Values(true, false)] bool fallback)
        {
            Assert.That(ChoiceKeys.ChoiceToBoolean(input, fallback), Is.EqualTo(fallback));
        }

        /// <summary>
        ///     Tests that invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with <see cref="ChoiceKeys.No" />
        ///     returns <code>false</code>.
        /// </summary>
        /// <param name="fallback">The fallback to test against.</param>
        [Test]
        public void TestChoiceToBoolean_NoIsFalse([Values(true, false)] bool fallback)
        {
            Assert.That(ChoiceKeys.ChoiceToBoolean(ChoiceKeys.No, fallback), Is.False);
        }

        /// <summary>
        ///     Tests that invocations of <see cref="ChoiceKeys.ChoiceToBoolean" /> with <see cref="ChoiceKeys.Yes" />
        ///     returns <code>true</code>.
        /// </summary>
        /// <param name="fallback">The fallback to test against.</param>
        [Test]
        public void TestChoiceToBoolean_YesIsTrue([Values(true, false)] bool fallback)
        {
            Assert.That(ChoiceKeys.ChoiceToBoolean(ChoiceKeys.Yes, fallback), Is.True);
        }
    }
}