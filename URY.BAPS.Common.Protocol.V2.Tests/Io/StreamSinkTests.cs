using System;
using System.IO;
using URY.BAPS.Common.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise <see cref="StreamSink" /> in isolation.
    /// </summary>
    public class StreamSinkTests
    {
        /// <summary>
        ///     Tests that constructing a <see cref="StreamSink" /> with a closed stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_ClosedStream()
        {
            var stream = new MemoryStream();
            stream.Close();

            Assert.Throws<ArgumentException>("stream", () => new StreamSink(stream));
        }

        /// <summary>
        ///     Tests that constructing a <see cref="StreamSink" /> with a null stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_NullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new StreamSink(null));
        }
    }
}