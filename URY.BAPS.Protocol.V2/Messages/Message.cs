﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Protocol.V2.Messages
{
    /// <summary>
    ///     A BapsNet message.
    /// </summary>
    public class Message
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
        public Message(ICommand cmd)
        {
            _cmd = cmd;
        }

        /// <summary>
        ///     Adds a string to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(string value)
        {
            return Add(new StringArgument {Value = value});
        }

        /// <summary>
        ///     Adds an unsigned integer to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(uint value)
        {
            return Add(new UintArgument {Value = value});
        }

        /// <summary>
        ///     Adds an float to this message.
        /// </summary>
        /// <param name="value">The float to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(float value)
        {
            return Add(new FloatArgument {Value = value});
        }

        /// <summary>
        ///     Sends this message to a sink, flushing at the end.
        /// </summary>
        /// <param name="sink">The (non-null) sink to send onto.</param>
        public void Send(ISink sink)
        {
            SendCommand(sink);
            SendLength(sink);
            SendArguments(sink);
            sink.Flush();
        }

        /// <summary>
        ///     Sends this message's command to the given sink.
        /// </summary>
        /// <param name="sink">The (non-null) sink to send onto.</param>
        private void SendCommand(ISink sink)
        {
            sink.SendCommand(_cmd.Packed);
        }

        /// <summary>
        ///     Sends each argument in this message to the given sink.
        /// </summary>
        /// <param name="sink">The (non-null) sink to send onto.</param>
        private void SendArguments(ISink sink)
        {
            foreach (var arg in _arguments) arg.Send(sink);
        }

        private void SendLength(ISink sock)
        {
            var length = (from arg in _arguments select arg.Length).Sum();
            Debug.Assert(0 <= length, "negative length");
            sock.SendUint((uint) length);
        }

        private Message Add(IArgument arg)
        {
            _arguments.Enqueue(arg);
            return this;
        }
    }
}