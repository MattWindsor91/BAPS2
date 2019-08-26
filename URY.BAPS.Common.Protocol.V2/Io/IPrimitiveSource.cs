using System.Threading;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     Low-level interface for objects that can produce items of the
    ///     primitive BapsNet types: commands, strings, floats, and uints.
    /// </summary>
    public interface IPrimitiveSource
    {
        /// <summary>
        ///     Performs a blocking (synchronous) read of a command word.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting 16-bit command word.</returns>
        ushort ReceiveCommand(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a string.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="string" />.</returns>
        string ReceiveString(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a float.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="float" />.</returns>
        float ReceiveFloat(CancellationToken token = default);

        /// <summary>
        ///     Performs a blocking (synchronous) read of a 32-bit unsigned integer.
        /// </summary>
        /// <param name="token">An optional cancellation token that can be used to abort the receive.</param>
        /// <returns>The resulting <see cref="uint" />.</returns>
        uint ReceiveUint(CancellationToken token = default);
    }
}