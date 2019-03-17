using System.Threading;

namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     Low-level interface for objects that can produce items of the
    ///     primitive BapsNet types: commands, strings, floats, and uints.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        ///     Performs a blocking (synchronous) read of a command word.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="Command"/>.</returns>
        Command ReceiveCommand(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a string
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="string"/>.</returns>
        string ReceiveString(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a float.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="float"/>.</returns>
        float ReceiveFloat(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a 32-bit unsigned integer.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="uint"/>.</returns>
        uint ReceiveUint(CancellationToken token = default);
    }
}