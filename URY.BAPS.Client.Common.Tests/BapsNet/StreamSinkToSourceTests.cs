using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using NUnit.Framework;
using URY.BAPS.Client.Common.BapsNet;

namespace URY.BAPS.Client.Common.Tests.BapsNet
{
    /// <summary>
    ///     Tests that exercise both <see cref="StreamSink"/> and <see cref="StreamSource"/>,
    ///     by applying them to the same stream.
    /// </summary>
    [TestFixture]
    public class StreamSinkToSourceTests
    {
        [CanBeNull] private StreamSink _sink;
        [CanBeNull] private StreamSource _source;
        [NotNull] private readonly Stream _stream = new MemoryStream();

        [SetUp]
        public void SetUp()
        {
            _stream.Seek(0, SeekOrigin.Begin);
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

        [Test]
        public void TestSendAndReceiveCommand()
        {
            const Command expectedCommand = Command.System | Command.AutoUpdate;
            _sink?.SendCommand(expectedCommand);
            Debug.Assert(_source != null, nameof(_source) + " != null");
            var actualCommand = SeekAndReceive(_source.ReceiveCommand);
            Assert.That(actualCommand, Is.EqualTo(expectedCommand));
        }

        [Test]
        public void TestSendAndReceiveString()
        {
            const string expectedString = "It's the end of the world as we know it (and I feel fine).";
            _sink?.SendString(expectedString);
            Debug.Assert(_source != null, nameof(_source) + " != null");
            var actualString = SeekAndReceive(_source.ReceiveString);
            Assert.That(actualString, Is.EqualTo(expectedString));
        }

        [Test]
        public void TestSendAndReceiveUint()
        {
            const uint expectedUint = 867_5309;
            _sink?.SendUint(expectedUint);
            Debug.Assert(_source != null, nameof(_source) + " != null");
            var actualUint = SeekAndReceive(_source.ReceiveUint);
            Assert.That(actualUint, Is.EqualTo(expectedUint));
        }

        [Test]
        public void TestSendAndReceiveFloat()
        {
            const float expectedFloat = 1024.25f;
            _sink?.SendFloat(expectedFloat);
            Debug.Assert(_source != null, nameof(_source) + " != null");
            var actualFloat = SeekAndReceive(_source.ReceiveFloat);
            Assert.That(actualFloat, Is.EqualTo(expectedFloat).Within(float.Epsilon));
        }
    }
}