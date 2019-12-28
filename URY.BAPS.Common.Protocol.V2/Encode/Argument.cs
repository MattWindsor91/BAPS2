using System.Globalization;
using System.Text;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Common.Protocol.V2.Encode
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

        void Send(IPrimitiveSink sock);
    }

    /// <summary>
    ///     A message argument representing a string.
    /// </summary>
    internal struct StringArgument : IArgument
    {
        public string Value;

        public int Length => Encoding.UTF8.GetByteCount(Value) + sizeof(uint);

        public void Send(IPrimitiveSink sock)
        {
            sock.SendString(Value);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }

    /// <summary>
    ///     A message argument representing a 32-bit integer.
    /// </summary>
    internal struct UintArgument : IArgument
    {
        public uint Value;

        public int Length => sizeof(uint);

        public void Send(IPrimitiveSink sock)
        {
            sock.SendUint(Value);
        }

        public override string ToString()
        {
            return $"{Value}U";
        }
    }

    /// <summary>
    ///     A message argument representing a single-precision float.
    /// </summary>
    internal struct FloatArgument : IArgument
    {
        public float Value;

        public int Length => sizeof(float);

        public void Send(IPrimitiveSink sock)
        {
            sock.SendFloat(Value);
        }

        public override string ToString()
        {
            return $"{Value}F";
        }
    }
}