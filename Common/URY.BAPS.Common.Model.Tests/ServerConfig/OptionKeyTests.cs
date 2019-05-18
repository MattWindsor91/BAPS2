using System;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.ServerConfig;
using Xunit;

namespace URY.BAPS.Common.Model.Tests.ServerConfig
{
    /// <summary>
    ///     Tests various <see cref="OptionKey" /> extensions and related code.
    /// </summary>
    public class OptionKeyTests
    {
        public static TheoryData<ChannelFlag> FlagData =>
            new TheoryData<ChannelFlag> {ChannelFlag.AutoAdvance, ChannelFlag.PlayOnLoad};

        /// <summary>
        ///     Tests that converting a channel flag to an option key and back is the identity.
        /// </summary>
        /// <param name="flag">The flag to test.</param>
        [Theory]
        [MemberData(nameof(FlagData))]
        public void TestChannelFlag_ToOptionKey_RoundTrip(ChannelFlag flag)
        {
            Assert.Equal(flag, flag.ToOptionKey().ToChannelFlag());
        }

        /// <summary>
        ///     Tests that converting a not-channel-related option key to a channel flag throws an exception.
        /// </summary>
        [Fact]
        public void TestToChannelFlag_NotChannelOptionKey()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => OptionKey.Port.ToChannelFlag());
        }
    }
}