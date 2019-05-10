using System;
using System.Text;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;
using Xunit;

namespace URY.BAPS.Common.Protocol.V2.Tests.Encode
{
    /// <summary>
    ///     Tests that <see cref="MessageBuilder" />'s construction and sending methods behave properly.
    /// </summary>
    public class MessageTests
    {
        private static void AssertMessage(MessageBuilder messageBuilder, ICommand expectedCommand, uint expectedLength,
            params Action<object>[] elementInspectors)
        {
            var sink = new DebugSink();
            messageBuilder.Send(sink);

            var finalElementInspectors = new Action<object>[elementInspectors.Length + 2];
            finalElementInspectors[0] = actualCmd =>
                Assert.Equal(expectedCommand.Packed, Assert.IsAssignableFrom<CommandWord>(actualCmd));
            finalElementInspectors[1] = length =>
                Assert.Equal(expectedLength, Assert.IsAssignableFrom<uint>(length));
            Array.Copy(elementInspectors, 0, finalElementInspectors, 2, elementInspectors.Length);

            Assert.Collection(sink.Items, finalElementInspectors);
        }

        private void AssertEqualUint(uint expected, object actual)
        {
            var actualUint = Assert.IsAssignableFrom<uint>(actual);
            Assert.Equal(expected, actualUint);
        }

        public static TheoryData<string> StringSystemCommandSendData =>
            new TheoryData<string> {"", "The system is down.", "バップス"};

        [Theory]
        [MemberData(nameof(StringSystemCommandSendData))]
        public void TestStringSystemCommandSend(string value)
        {
            var cmd = new SystemCommand(SystemOp.SendLogMessage);

            // Strings are UTF-8, so the command length is equal to the number of UTF-8 bytes in the value, plus four
            // characters for the on-wire representation of the string's length.
            var valueLength = Encoding.UTF8.GetByteCount(value);
            var expectedLength = (uint) valueLength + 4;

            var m = new MessageBuilder(new SystemCommand(SystemOp.SendLogMessage)).Add(value);

            AssertMessage(m, cmd, expectedLength,
                actualValue => Assert.Equal(value, Assert.IsAssignableFrom<string>(actualValue)));
        }

        /// <summary>
        ///     Tests sending a command with a floating-point argument.
        /// </summary>
        [Fact]
        public void TestFloatChannelCommandSend()
        {
            var cmd = new PlaybackCommand(PlaybackOp.Volume, 0);
            const float value = 0.75f;

            var m = new MessageBuilder(cmd).Add(value);

            // Floats are 32-bit, so the length should be 4.
            const uint expectedLength = 4;

            AssertMessage(m, cmd, expectedLength,
                actualValue => Assert.Equal(value, Assert.IsAssignableFrom<float>(actualValue), 2));
        }

        /// <summary>
        ///     Tests sending a command with an unsigned integer argument.
        /// </summary>
        [Fact]
        public void TestU32ChannelCommandSend()
        {
            var cmd = new PlaybackCommand(PlaybackOp.CuePosition, 0);
            const uint value = 3600;

            var m = new MessageBuilder(cmd).Add(value);

            // Being one 32-bit argument, the length should be 4.
            const uint expectedLength = 4;

            AssertMessage(m, cmd, expectedLength,
                actualValue => AssertEqualUint(value, actualValue));
        }

        /// <summary>
        ///     Tests sending a command with zero trailing arguments, but an inline channel argument.
        /// </summary>
        [Fact]
        public void TestZeroArgumentChannelCommandSend()
        {
            var cmd = new PlaybackCommand(PlaybackOp.Play, 42);
            var m = new MessageBuilder(cmd);
            // Even zero-argument commands have a trailing length (of 0).
            const uint expectedLength = 0;
            AssertMessage(m, cmd, expectedLength);
        }


        /// <summary>
        ///     Tests sending a command with zero arguments.
        /// </summary>
        [Fact]
        public void TestZeroArgumentCommandSend()
        {
            var cmd = new SystemCommand(SystemOp.Quit);
            var m = new MessageBuilder(cmd);
            // Even zero-argument commands have a trailing length (of 0).
            const uint expectedLength = 0;
            AssertMessage(m, cmd, expectedLength);
        }
    }
}