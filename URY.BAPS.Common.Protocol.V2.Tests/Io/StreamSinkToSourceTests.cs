using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise both <see cref="StreamSink" /> and <see cref="StreamBapsNetSource" />,
    ///     by applying them to the same stream.
    /// </summary>
    public class StreamSinkToSourceTests
    {
        public StreamSinkToSourceTests()
        {
            _sink = new StreamSink(_stream);
            _bapsNetSource = new StreamBapsNetSource(_stream);
        }

        [NotNull] private readonly StreamSink _sink;
        [NotNull] private readonly StreamBapsNetSource _bapsNetSource;
        [NotNull] private readonly BufferedStream _stream = new BufferedStream(new MemoryStream());

        private T SeekAndReceive<T>(Func<CancellationToken, T> receive)
        {
            _stream.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
            var cts = new CancellationTokenSource(1_000);
            return receive(cts.Token);
        }

        [Fact]
        public void TestSendAndReceiveCommand()
        {
            const CommandWord expectedCommand = CommandWord.System | CommandWord.AutoUpdate;
            _sink.SendCommand(expectedCommand);
            _sink.Flush();
            var actualCommand = SeekAndReceive(_bapsNetSource.ReceiveCommand);
            Assert.Equal(expectedCommand, actualCommand);
        }

        [Fact]
        public void TestSendAndReceiveFloat()
        {
            const float expectedFloat = 1024.25f;
            _sink.SendFloat(expectedFloat);
            var actualFloat = SeekAndReceive(_bapsNetSource.ReceiveFloat);
            Assert.Equal(expectedFloat, actualFloat, 2);
        }

        [Fact]
        public void TestSendAndReceiveString()
        {
            const string expectedString = "It's the end of the world as we know it (and I feel fine).";
            _sink.SendString(expectedString);
            var actualString = SeekAndReceive(_bapsNetSource.ReceiveString);
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
            using (var sink = new StreamSink(_stream))
            {
                sink.SendString(expectedString);
            }

            _stream.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
            using var source = new StreamBapsNetSource(_stream);
            var actualString = source.ReceiveString();
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void TestSendAndReceiveUint()
        {
            const uint expectedUint = 867_5309;
            _sink.SendUint(expectedUint);
            var actualUint = SeekAndReceive(_bapsNetSource.ReceiveUint);
            Assert.Equal(expectedUint, actualUint);
        }
    }
}