using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace BAPSCommon
{
    /// <summary>
    /// A BAPSNet message.
    /// </summary>
    public class Message
    {
        private interface IArgument
        {
            int Length();
            void Send(ClientSocket sock);
        }

        private struct SArgument : IArgument
        {
            public string value;
            public int Length() => System.Text.Encoding.UTF8.GetByteCount(value) + sizeof(uint);
            public void Send(ClientSocket sock) => sock.Send(value);
        }

        private struct U32Argument : IArgument
        {
            public uint value;
            public int Length() => sizeof(uint);
            public void Send(ClientSocket sock) => sock.Send(value);
        }

        private struct FArgument : IArgument
        {
            public float value;
            public int Length() => sizeof(float);
            public void Send(ClientSocket sock) => sock.Send(value);
        }

        private readonly Command cmd;
        private Queue<IArgument> args = new Queue<IArgument>();

        /// <summary>
        /// Constructs a message containing the given command word.
        /// </summary>
        /// <param name="_cmd">The command word.</param>
        public Message(Command _cmd)
        {
            cmd = _cmd;
        }

        /// <summary>
        /// Adds a string to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(string value) => Add(new SArgument { value = value });

        /// <summary>
        /// Adds an unsigned integer to this message.
        /// </summary>
        /// <param name="value">The string to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(uint value) => Add(new U32Argument { value = value });


        /// <summary>
        /// Adds an float to this message.
        /// </summary>
        /// <param name="value">The float to add to the message.</param>
        /// <returns>This.</returns>
        public Message Add(float value) => Add(new FArgument { value = value });

        /// <summary>
        /// Sends this command over a client socket.
        /// </summary>
        /// <param name="sock">The socket to send onto.</param>
        public void Send(ClientSocket sock)
        {
            sock.Send(cmd);
            SendLength(sock);
            foreach (var arg in args) arg.Send(sock);
        }

        private void SendLength(ClientSocket sock)
        {
            int length = (from arg in args select arg.Length()).Sum();
            Debug.Assert(0 <= length, "negative length");
            sock.Send((uint)length);
        }

        private Message Add(IArgument arg)
        {
            args.Enqueue(arg);
            return this;
        }
    }
}
