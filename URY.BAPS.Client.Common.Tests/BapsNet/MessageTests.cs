using System.Text;
using NUnit.Framework;
using URY.BAPS.Client.Common.BapsNet;

namespace URY.BAPS.Client.Common.Tests.BapsNet
{
    /// <summary>
    ///     Tests that <see cref="Message" />'s construction and sending methods behave properly.
    /// </summary>
    [TestFixture]
    public class MessageTests
    {
        [SetUp]
        public void SetUp()
        {
            _sink.Clear();
        }

        /// <summary>
        ///     A sink that just collects the raw BapsNet primitives sent to it.
        /// </summary>
        private readonly DebugSink _sink = new DebugSink();

        /// <summary>
        ///     Tests sending a command with a floating-point argument.
        /// </summary>
        [Test]
        public void TestFloatChannelCommandSend()
        {
            const Command cmd = Command.Playback | Command.Volume;
            const float value = 0.75f;

            var m = new Message(cmd).Add(value);
            m.Send(_sink);

            var result = _sink.Items;
            Assert.That(result, Has.Length.EqualTo(3));

            var actualCmd = result[0];
            Assert.That(actualCmd, Is.InstanceOf<Command>().And.EqualTo(cmd));

            // Floats are 32-bit, so the length should be 4.
            var length = result[1];
            Assert.That(length, Is.InstanceOf<uint>().And.EqualTo(4));

            var actualValue = result[2];
            Assert.That(actualValue, Is.InstanceOf<float>().And.EqualTo(value).Within(1).Percent);
        }

        [Test]
        [Combinatorial]
        public void TestStringSystemCommandSend([Values("", "The system is down.", "バップス")]
            string value)
        {
            const Command cmd = Command.System | Command.SendLogMessage;

            var m = new Message(cmd).Add(value);
            m.Send(_sink);

            var result = _sink.Items;
            Assert.That(result, Has.Length.EqualTo(3));

            var actualCmd = result[0];
            Assert.That(actualCmd, Is.InstanceOf<Command>().And.EqualTo(cmd));

            // Strings are UTF-8, so the command length is equal to the number of UTF-8 bytes in the value, plus four
            // characters for the on-wire representation of the string's length.
            var valueLength = Encoding.UTF8.GetByteCount(value);
            var length = result[1];
            Assert.That(length, Is.InstanceOf<uint>().And.EqualTo(valueLength + 4));

            var actualValue = result[2];
            Assert.That(actualValue, Is.InstanceOf<string>().And.EqualTo(value));
        }

        /// <summary>
        ///     Tests sending a command with an unsigned integer argument.
        /// </summary>
        [Test]
        public void TestU32ChannelCommandSend()
        {
            const Command cmd = Command.Playback | Command.CuePosition;
            const uint value = 3600;

            var m = new Message(cmd).Add(value);
            m.Send(_sink);

            var result = _sink.Items;
            Assert.That(result, Has.Length.EqualTo(3));

            var actualCmd = result[0];
            Assert.That(actualCmd, Is.InstanceOf<Command>().And.EqualTo(cmd));

            // Being one 32-bit argument, the length should be 4.
            var length = result[1];
            Assert.That(length, Is.InstanceOf<uint>().And.EqualTo(4));

            var actualValue = result[2];
            Assert.That(actualValue, Is.InstanceOf<uint>().And.EqualTo(value));
        }

        /// <summary>
        ///     Tests sending a command with zero trailing arguments, but an inline channel argument.
        /// </summary>
        [Test]
        public void TestZeroArgumentChannelCommandSend()
        {
            var cmd = (Command.Playback | Command.Play).WithChannel(42);

            var m = new Message(cmd);
            m.Send(_sink);

            var result = _sink.Items;
            Assert.That(result, Has.Length.EqualTo(2));

            var actualCmd = result[0];
            Assert.That(actualCmd, Is.InstanceOf<Command>().And.EqualTo(cmd));

            var length = result[1];
            Assert.That(length, Is.InstanceOf<uint>().And.EqualTo(0));
        }


        /// <summary>
        ///     Tests sending a command with zero arguments.
        /// </summary>
        [Test]
        public void TestZeroArgumentCommandSend()
        {
            const Command cmd = Command.System | Command.Quit;

            var m = new Message(cmd);
            m.Send(_sink);

            var result = _sink.Items;
            Assert.That(result, Has.Length.EqualTo(2));

            var actualCmd = result[0];
            Assert.That(actualCmd, Is.InstanceOf<Command>().And.EqualTo(cmd));

            // Even zero-argument commands have a trailing length (of 0).
            var length = result[1];
            Assert.That(length, Is.InstanceOf<uint>().And.EqualTo(0));
        }
    }
}