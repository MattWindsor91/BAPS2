namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     Low-level interface for objects that can receive items of the
    ///     primitive BAPSNet types: commands, strings, floats, and uints.
    /// </summary>
    public interface ISink
    {
        /// <summary>
        ///     Sends a command down this sink.
        /// </summary>
        /// <param name="cmd">The command to send.</param>
        void SendCommand(Command cmd);

        /// <summary>
        ///     Sends a string down this sink.
        /// </summary>
        /// <param name="s">The string to send.</param>
        void SendString(string s);

        /// <summary>
        ///     Sends a float down this sink.
        /// </summary>
        /// <param name="f">The float to send.</param>
        void SendFloat(float f);

        /// <summary>
        ///     Sends a 32-bit unsigned integer down this sink.
        /// </summary>
        /// <param name="i">The integer to send.</param>
        void SendU32(uint i);
    }
}