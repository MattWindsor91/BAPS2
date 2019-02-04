using BAPSClientCommon.BapsNet;
using NUnit.Framework;

namespace BAPSClientCommonTests.BapsNet
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
    }
}