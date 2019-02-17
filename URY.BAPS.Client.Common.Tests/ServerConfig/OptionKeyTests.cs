using System;
using NUnit.Framework;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Common.Tests.ServerConfig
{
    /// <summary>
    ///     Tests various <see cref="OptionKey" /> extensions and related code.
    /// </summary>
    [TestFixture]
    public class OptionKeyTests
    {
        /// <summary>
        ///     Tests that converting a channel flag to an option key and back is the identity.
        /// </summary>
        /// <param name="flag">The flag to test.</param>
        [Test]
        public void TestChannelFlag_ToOptionKey_RoundTrip([Values(ChannelFlag.AutoAdvance, ChannelFlag.PlayOnLoad)]
            ChannelFlag flag)
        {
            Assert.That(flag.ToOptionKey().ToChannelFlag(), Is.EqualTo(flag));
        }

        /// <summary>
        ///     Tests that converting a not-channel-related option key to a channel flag throws an exception.
        /// </summary>
        [Test]
        public void TestToChannelFlag_NotChannelOptionKey()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => OptionKey.Port.ToChannelFlag());
        }
    }
}