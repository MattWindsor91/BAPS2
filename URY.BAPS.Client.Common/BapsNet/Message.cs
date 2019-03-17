using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     A BapsNet message.
    /// </summary>
    public class Message
    {
        [NotNull] private readonly Queue<IArgument> _args = new Queue<IArgument>();

        private readonly Command _cmd;

        /// <summary>
        ///     Constructs a message containing the given command word.
        /// </summary>
        /// <param name="cmd">The command word.</param>
        public Message(Command cmd)
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
        ///     Sends this command to a sink.
        /// </summary>
        /// <param name="sink">The socket to send onto.</param>
        public void Send(ISink sink)
        {
            sink.SendCommand(_cmd);
            SendLength(sink);
            foreach (var arg in _args) arg.Send(sink);
        }

        private void SendLength(ISink sock)
        {
            var length = (from arg in _args select arg.Length()).Sum();
            Debug.Assert(0 <= length, "negative length");
            sock.SendUint((uint) length);
        }

        private Message Add(IArgument arg)
        {
            _args.Enqueue(arg);
            return this;
        }

        private interface IArgument
        {
            [Pure]
            int Length();

            void Send(ISink sock);
        }

        private struct StringArgument : IArgument
        {
            public string Value;

            [Pure]
            public int Length()
            {
                return Encoding.UTF8.GetByteCount(Value) + sizeof(uint);
            }

            public void Send(ISink sock)
            {
                sock.SendString(Value);
            }
        }

        private struct UintArgument : IArgument
        {
            public uint Value;

            [Pure]
            public int Length()
            {
                return sizeof(uint);
            }

            public void Send(ISink sock)
            {
                sock.SendUint(Value);
            }
        }

        private struct FloatArgument : IArgument
        {
            public float Value;

            [Pure]
            public int Length()
            {
                return sizeof(float);
            }

            public void Send(ISink sock)
            {
                sock.SendFloat(Value);
            }
        }
    }
}