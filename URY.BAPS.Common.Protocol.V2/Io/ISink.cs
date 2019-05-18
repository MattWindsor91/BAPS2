using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     Low-level interface for objects that can consume items of the
    ///     primitive BapsNet types: commands, strings, floats, and uints.
    /// </summary>
    public interface ISink
    {
        /// <summary>
        ///     Sends a command down this sink.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        /// <seealso cref="Flush" />
        void SendCommand(CommandWord cmd);

        /// <summary>
        ///     Sends a string down this sink.
        /// </summary>
        /// <param name="s">The string to send.</param>
        /// <seealso cref="Flush" />
        void SendString(string s);

        /// <summary>
        ///     Sends a float down this sink.
        /// </summary>
        /// <param name="f">The float to send.</param>
        /// <seealso cref="Flush" />
        void SendFloat(float f);

        /// <summary>
        ///     Sends a 32-bit unsigned integer down this sink.
        /// </summary>
        /// <param name="i">The integer to send.</param>
        /// <seealso cref="Flush" />
        void SendUint(uint i);

        /// <summary>
        ///     Forces the sink to propagate any sent BapsNet primitives.
        /// </summary>
        void Flush();
    }
}