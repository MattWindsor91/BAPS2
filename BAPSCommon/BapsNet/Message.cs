using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BAPSClientCommon.BapsNet
{
    /// <summary>
    ///     A BAPSNet message.
    /// </summary>
    public class Message
    {
        private readonly Queue<IArgument> _args = new Queue<IArgument>();

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
            return Add(new SArgument {Value = value});
        }

        /// <summary>
        ///     Adds an unsigned integer to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(uint value)
        {
            return Add(new U32Argument {Value = value});
        }


        /// <summary>
        ///     Adds an float to this message.
        /// </summary>
        /// <param name="value">The float to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(float value)
        {
            return Add(new FArgument {Value = value});
        }

        /// <summary>
        ///     Sends this command over a client socket.
        /// </summary>
        /// <param name="sock">The socket to send onto.</param>
        public void Send(ISink sock)
        {
            sock.SendCommand(_cmd);
            SendLength(sock);
            foreach (var arg in _args) arg.Send(sock);
        }

        private void SendLength(ISink sock)
        {
            var length = (from arg in _args select arg.Length()).Sum();
            Debug.Assert(0 <= length, "negative length");
            sock.SendU32((uint) length);
        }

        private Message Add(IArgument arg)
        {
            _args.Enqueue(arg);
            return this;
        }

        private interface IArgument
        {
            int Length();
            void Send(ISink sock);
        }

        private struct SArgument : IArgument
        {
            public string Value;

            public int Length()
            {
                return Encoding.UTF8.GetByteCount(Value) + sizeof(uint);
            }

            public void Send(ISink sock)
            {
                sock.SendString(Value);
            }
        }

        private struct U32Argument : IArgument
        {
            public uint Value;

            public int Length()
            {
                return sizeof(uint);
            }

            public void Send(ISink sock)
            {
                sock.SendU32(Value);
            }
        }

        private struct FArgument : IArgument
        {
            public float Value;

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