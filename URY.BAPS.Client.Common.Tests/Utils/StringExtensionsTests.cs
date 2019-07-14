using System;
using URY.BAPS.Client.Common.Utils;
using Xunit;

namespace URY.BAPS.Client.Common.Tests.Utils
{
    /// <summary>
    ///     Tests for the <see cref="StringExtensions" /> extension set.
    /// </summary>
    public class StringExtensionsTests
    {
        /// <summary>
        ///     Tests that a zero custom length causes an exception.
        /// </summary>
        [Fact]
        public void TestSummary_InvalidCustomLength()
        {
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => "foo".Summary(0));
        }

        /// <summary>
        ///     Tests that a long string is shortened, using a custom length.
        /// </summary>
        [Fact]
        public void TestSummary_LongStringCustomLength()
        {
            Assert.Equal("The quick brown fox jumps\u2026", "The quick brown fox jumps over the lazy dog".Summary(26));
        }

        /// <summary>
        ///     Tests that a long string is shortened, using the default length.
        /// </summary>
        [Fact]
        public void TestSummary_LongStringWithDefaults()
        {
            Assert.Equal("The quick brown fox jumps over \u2026",
                "The quick brown fox jumps over the lazy dog".Summary());
        }

        /// <summary>
        ///     Tests that a short string isn't shortened, using a custom edge-case length.
        /// </summary>
        [Fact]
        public void TestSummary_ShortStringCustomLength()
        {
            Assert.Equal("foo", "foo".Summary(3));
        }

        /// <summary>
        ///     Tests that a short string isn't shortened, using the default length.
        /// </summary>
        [Fact]
        public void TestSummary_ShortStringWithDefaults()
        {
            Assert.Equal("foo", "foo".Summary());
        }
    }
}