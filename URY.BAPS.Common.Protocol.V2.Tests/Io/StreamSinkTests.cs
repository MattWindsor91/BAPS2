using System;
using System.IO;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise <see cref="StreamPrimitiveSink" /> in isolation.
    /// </summary>
    public class StreamSinkTests
    {
        /// <summary>
        ///     Tests that constructing a <see cref="StreamPrimitiveSink" /> with a closed stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_ClosedStream()
        {
            var stream = new MemoryStream();
            stream.Close();

            Assert.Throws<ArgumentException>("stream", () => new StreamPrimitiveSink(stream));
        }

        /// <summary>
        ///     Tests that constructing a <see cref="StreamPrimitiveSink" /> with a null stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_NullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new StreamPrimitiveSink(null));
        }
    }
}