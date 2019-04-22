using System.Text;
using JetBrains.Annotations;
using URY.BAPS.Protocol.V2.Io;

namespace URY.BAPS.Protocol.V2.Messages
{
    /// <summary>
    ///     Interface for message arguments.
    /// </summary>
    internal interface IArgument
    {
        /// <summary>
        ///     The length of the argument, in bytes.
        /// </summary>
        int Length { get; }

        void Send(ISink sock);
    }

    /// <summary>
    ///     A message argument representing a string.
    /// </summary>
    internal struct StringArgument : IArgument
    {
        public string Value;

        public int Length => Encoding.UTF8.GetByteCount(Value) + sizeof(uint);

        public void Send(ISink sock)
        {
            sock.SendString(Value);
        }
    }

    /// <summary>
    ///     A message argument representing a 32-bit integer.
    /// </summary>
    internal struct UintArgument : IArgument
    {
        public uint Value;

        public int Length => sizeof(uint);

        public void Send(ISink sock)
        {
            sock.SendUint(Value);
        }
    }

    /// <summary>
    ///     A message argument representing a single-precision float.
    /// </summary>
    internal struct FloatArgument : IArgument
    {
        public float Value;

        public int Length => sizeof(float);

        public void Send(ISink sock)
        {
            sock.SendFloat(Value);
        }
    }
}