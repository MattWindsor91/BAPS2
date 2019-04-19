using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     Takes requests to send BapsNet primitives, and applies them to a
    ///     <see cref="Stream"/> of bytes.
    /// </summary>
    public class StreamSink : ISink
    {
        [NotNull] private readonly Stream _stream;

        public StreamSink([CanBeNull] Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc cref="ISink"/>
        public void SendCommand(CommandWord cmd)
        {
            SendNetworkOrder(BitConverter.GetBytes((ushort)cmd));
        }

        /// <inheritdoc cref="ISink"/>
        public void SendString(string s)
        {
            SendUint((uint)Encoding.UTF8.GetByteCount(s));
            Send(Encoding.UTF8.GetBytes(s));
        }

        /// <inheritdoc cref="ISink"/>
        public void SendFloat(float f)
        {
            SendNetworkOrder(BitConverter.GetBytes(f));
        }

        /// <inheritdoc cref="ISink"/>
        public void SendUint(uint i)
        {
            SendNetworkOrder(BitConverter.GetBytes(i));
        }

        /// <summary>
        ///     Sends all given data to the stream synchronously, in
        ///     network (big-endian) order.
        ///     <para>
        ///         If the host architecture is little-endian, the contents
        ///         of <paramref name="bytes"/> will be reversed in-place.
        ///     </para>
        /// </summary>
        /// <param name="bytes">The data to send (in its entirety).</param>
        private void SendNetworkOrder(byte[] bytes)
        {
            BitManipulation.ShuffleForNetworkOrder(bytes, bytes.Length);
            Send(bytes);
        }

        /// <summary>
        ///     Sends all given data to the stream synchronously.
        /// </summary>
        /// <param name="bytes">The data to send (in its entirety).</param>
        [Pure] private void Send(byte[] bytes)
        {
            _stream.Write(bytes, 0, bytes.Length);
        }
    }
}