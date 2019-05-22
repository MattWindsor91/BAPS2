using System;
using System.IO;
using URY.BAPS.Common.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise <see cref="StreamPrimitiveSource" /> in isolation.
    /// </summary>
    public class StreamSourceTests
    {
        /// <summary>
        ///     Tests that constructing a <see cref="StreamPrimitiveSource" /> with a closed stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_ClosedStream()
        {
            var stream = new MemoryStream();
            stream.Close();

            Assert.Throws<ArgumentException>("stream", () => new StreamPrimitiveSource(stream));
        }

        /// <summary>
        ///     Tests that constructing a <see cref="StreamPrimitiveSource" /> with a null stream fails.
        /// </summary>
        [Fact]
        public void TestConstruct_NullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new StreamPrimitiveSource(null));
        }
    }
}