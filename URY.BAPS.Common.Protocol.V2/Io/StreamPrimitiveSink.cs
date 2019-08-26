using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     Takes requests to send BapsNet primitives, and applies them to a
    ///     <see cref="Stream" /> of bytes.
    ///     <para>
    ///         Disposing the <see cref="StreamPrimitiveSink" /> does NOT dispose the underlying stream.
    ///     </para>
    /// </summary>
    public class StreamPrimitiveSink : IPrimitiveSink, IDisposable
    {
        /// <summary>
        ///     A writer used to send bytes to a stream.
        /// </summary>
        [NotNull] private readonly BinaryWriter _writer;

        public StreamPrimitiveSink(Stream? stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new ArgumentException("Stream must be readable", nameof(stream));
            _writer = new BinaryWriter(stream, Encoding.UTF8, true);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        /// <inheritdoc cref="IPrimitiveSink" />
        public void SendCommand(ushort cmd)
        {
            SendNetworkOrder(BitConverter.GetBytes((ushort) cmd));
        }

        /// <inheritdoc cref="IPrimitiveSink" />
        public void SendString(string s)
        {
            SendUint((uint) Encoding.UTF8.GetByteCount(s));
            _writer.Write(s.ToCharArray());
        }

        /// <inheritdoc cref="IPrimitiveSink" />
        public void SendFloat(float f)
        {
            SendNetworkOrder(BitConverter.GetBytes(f));
        }

        /// <inheritdoc cref="IPrimitiveSink" />
        public void SendUint(uint i)
        {
            SendNetworkOrder(BitConverter.GetBytes(i));
        }

        public void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        ///     Sends all given data to the stream synchronously, in
        ///     network (big-endian) order.
        ///     <para>
        ///         If the host architecture is little-endian, the contents
        ///         of <paramref name="bytes" /> will be reversed in-place.
        ///     </para>
        /// </summary>
        /// <param name="bytes">The data to send (in its entirety).</param>
        private void SendNetworkOrder(byte[] bytes)
        {
            BitManipulation.ShuffleForNetworkOrder(bytes, bytes.Length);
            _writer.Write(bytes);
        }
    }
}