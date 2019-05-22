using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Common.Protocol.V2.Encode
{
    /// <summary>
    ///     A BapsNet packed message builder.
    ///     <para>
    ///         This class is useful for building up the line representation of a
    ///         BapsNet message before sending it.  As BapsNet messages aren't
    ///         self-describing, this class isn't useful for decoding messages.
    ///     </para>
    /// </summary>
    public class MessageBuilder
    {
        /// <summary>
        ///     Queue of arguments added to this message.
        /// </summary>
        [NotNull] private readonly Queue<IArgument> _arguments = new Queue<IArgument>();

        private readonly ICommand _cmd;

        /// <summary>
        ///     Constructs a message containing the given command.
        /// </summary>
        /// <param name="cmd">The command word.</param>
        public MessageBuilder(ICommand cmd)
        {
            _cmd = cmd;
        }

        /// <summary>
        ///     Adds a string to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public MessageBuilder Add(string value)
        {
            return Add(new StringArgument {Value = value});
        }

        /// <summary>
        ///     Adds an unsigned integer to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public MessageBuilder Add(uint value)
        {
            return Add(new UintArgument {Value = value});
        }

        /// <summary>
        ///     Adds an float to this message.
        /// </summary>
        /// <param name="value">The float to add to the message.</param>
        /// <returns>This.</returns>
        public MessageBuilder Add(float value)
        {
            return Add(new FloatArgument {Value = value});
        }

        /// <summary>
        ///     Sends this message to a sink, flushing at the end.
        /// </summary>
        /// <param name="primitiveSink">The (non-null) sink to send onto.</param>
        public void Send(IPrimitiveSink primitiveSink)
        {
            SendCommand(primitiveSink);
            SendLength(primitiveSink);
            SendArguments(primitiveSink);
            primitiveSink.Flush();
        }

        /// <summary>
        ///     Sends this message's command to the given sink.
        /// </summary>
        /// <param name="primitiveSink">The (non-null) sink to send onto.</param>
        private void SendCommand(IPrimitiveSink primitiveSink)
        {
            primitiveSink.SendCommand(_cmd.Packed);
        }

        /// <summary>
        ///     Sends each argument in this message to the given sink.
        /// </summary>
        /// <param name="primitiveSink">The (non-null) sink to send onto.</param>
        private void SendArguments(IPrimitiveSink primitiveSink)
        {
            foreach (var arg in _arguments) arg.Send(primitiveSink);
        }

        private void SendLength(IPrimitiveSink sock)
        {
            var length = (from arg in _arguments select arg.Length).Sum();
            Debug.Assert(0 <= length, "negative length");
            sock.SendUint((uint) length);
        }

        private MessageBuilder Add(IArgument arg)
        {
            _arguments.Enqueue(arg);
            return this;
        }
    }
}