using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Io;
using URY.BAPS.Common.Protocol.V2.Ops;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise both <see cref="StreamPrimitiveSink" /> and <see cref="StreamPrimitiveSource" />,
    ///     by applying them to the same stream.
    /// </summary>
    public class StreamSinkToSourceTests
    {
        public StreamSinkToSourceTests()
        {
            _primitiveSink = new StreamPrimitiveSink(_stream);
            _primitiveSource = new StreamPrimitiveSource(_stream);
        }

        [NotNull] private readonly StreamPrimitiveSink _primitiveSink;
        [NotNull] private readonly StreamPrimitiveSource _primitiveSource;
        [NotNull] private readonly BufferedStream _stream = new BufferedStream(new MemoryStream());

        private T SeekAndReceive<T>(Func<CancellationToken, T> receive)
        {
            _stream.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
            using var cts = new CancellationTokenSource(1_000);
            return receive(cts.Token);
        }

        [Fact]
        public void TestSendAndReceiveCommand()
        {
            var expectedCommand = (ushort) (CommandGroup.System.ToWordBits() | SystemOp.AutoUpdate.ToWordBits());
            _primitiveSink.SendCommand(expectedCommand);
            _primitiveSink.Flush();
            var actualCommand = SeekAndReceive(_primitiveSource.ReceiveCommand);
            Assert.Equal(expectedCommand, actualCommand);
        }

        [Fact]
        public void TestSendAndReceiveFloat()
        {
            const float expectedFloat = 1024.25f;
            _primitiveSink.SendFloat(expectedFloat);
            var actualFloat = SeekAndReceive(_primitiveSource.ReceiveFloat);
            Assert.Equal(expectedFloat, actualFloat, 2);
        }

        [Fact]
        public void TestSendAndReceiveString()
        {
            const string expectedString = "It's the end of the world as we know it (and I feel fine).";
            _primitiveSink.SendString(expectedString);
            var actualString = SeekAndReceive(_primitiveSource.ReceiveString);
            Assert.Equal(expectedString, actualString);
        }

        /// <summary>
        ///     Like <see cref="TestSendAndReceiveString" />, but with a stream source and sink that we
        ///     explicitly dispose (to exercise their respective disposal methods).
        /// </summary>
        [Fact]
        public void TestSendAndReceiveString_Dispose()
        {
            const string expectedString = "LEONARD BERNSTEIN!";
            using (var sink = new StreamPrimitiveSink(_stream))
            {
                sink.SendString(expectedString);
            }

            _stream.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
            using var source = new StreamPrimitiveSource(_stream);
            var actualString = source.ReceiveString();
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void TestSendAndReceiveUint()
        {
            const uint expectedUint = 867_5309;
            _primitiveSink.SendUint(expectedUint);
            var actualUint = SeekAndReceive(_primitiveSource.ReceiveUint);
            Assert.Equal(expectedUint, actualUint);
        }
    }
}