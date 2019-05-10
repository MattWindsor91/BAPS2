using System;
using System.IO;
using URY.BAPS.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise <see cref="StreamBapsNetSource" /> in isolation.
    /// </summary>
    public class StreamSourceTests
    {
        /// <summary>
        ///     Tests that constructing a <see cref="StreamBapsNetSource" /> with a closed stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_ClosedStream()
        {
            var stream = new MemoryStream();
            stream.Close();

            Assert.Throws<ArgumentException>("stream", () => new StreamBapsNetSource(stream));
        }

        /// <summary>
        ///     Tests that constructing a <see cref="StreamBapsNetSource" /> with a null stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_NullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new StreamBapsNetSource(null));
        }
    }
}