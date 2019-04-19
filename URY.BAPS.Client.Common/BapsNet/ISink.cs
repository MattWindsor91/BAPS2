namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     Low-level interface for objects that can consume items of the
    ///     primitive BapsNet types: commands, strings, floats, and uints.
    /// </summary>
    public interface ISink
    {
        /// <summary>
        ///     Synchronously sends a command down this sink.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        void SendCommand(CommandWord cmd);

        /// <summary>
        ///     Synchronously sends a string down this sink.
        /// </summary>
        /// <param name="s">The string to send.</param>
        void SendString(string s);

        /// <summary>
        ///     Synchronously sends a float down this sink.
        /// </summary>
        /// <param name="f">The float to send.</param>
        void SendFloat(float f);

        /// <summary>
        ///     Synchronously sends a 32-bit unsigned integer down this sink.
        /// </summary>
        /// <param name="i">The integer to send.</param>
        void SendUint(uint i);
    }
}