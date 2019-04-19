using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Protocol.V2.Tests.Io
{
    /// <summary>
    ///     Tests that exercise both <see cref="StreamSink"/> and <see cref="StreamSource"/>,
    ///     by applying them to the same stream.
    /// </summary>
    public class StreamSinkToSourceTests
    {
        [NotNull] private readonly StreamSink _sink;
        [NotNull] private readonly StreamSource _source;
        [NotNull] private readonly Stream _stream = new MemoryStream();

        public StreamSinkToSourceTests()
        {
            _sink = new StreamSink(_stream);
            _source = new StreamSource(_stream);
        }

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
            Debug.Assert(_source != null, nameof(_source) + " != null");
            var actualCommand = SeekAndReceive(_source.ReceiveCommand);
            Assert.Equal(expectedCommand, actualCommand);
        }

        [Fact]
        public void TestSendAndReceiveString()
        {
            const string expectedString = "It's the end of the world as we know it (and I feel fine).";
            _sink.SendString(expectedString);
            var actualString = SeekAndReceive(_source.ReceiveString);
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void TestSendAndReceiveUint()
        {
            const uint expectedUint = 867_5309;
            _sink.SendUint(expectedUint);
            var actualUint = SeekAndReceive(_source.ReceiveUint);
            Assert.Equal(expectedUint, actualUint);
        }

        [Fact]
        public void TestSendAndReceiveFloat()
        {
            const float expectedFloat = 1024.25f;
            _sink.SendFloat(expectedFloat);
            var actualFloat = SeekAndReceive(_source.ReceiveFloat);
            Assert.Equal(expectedFloat, actualFloat, 2);
        }
    }
}