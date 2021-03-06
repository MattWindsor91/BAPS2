﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace URY.BAPS.Common.Protocol.V2.PrimitiveIo
{
    /// <summary>
    ///     Takes requests to receive BapsNet primitives, and applies them to a
    ///     <see cref="BufferedStream" /> of bytes.
    ///     <para>
    ///         Disposing the <see cref="StreamPrimitiveSink" /> does NOT dispose the underlying stream.
    ///     </para>
    /// </summary>
    public class StreamPrimitiveSource : IPrimitiveSource, IDisposable
    {
        [NotNull] private readonly BinaryReader _reader;

        public StreamPrimitiveSource(Stream? stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new ArgumentException("Stream must be readable", nameof(stream));
            _reader = new BinaryReader(stream, Encoding.UTF8, true);
        }

        public ushort ReceiveCommand(CancellationToken token = default)
        {
            return BitConverter.ToUInt16(ReceiveNetworkOrder(2, token), 0);
        }

        public string ReceiveString(CancellationToken token = default)
        {
            // The string length is in _bytes_, not characters, and is fixed as a 32-bit quantity.
            // This means we can't use _reader.ReadChars or _reader.ReadString.
            var stringLength = (int) ReceiveUint(token);
            var stringBytes = _reader.ReadBytes(stringLength);
            return Encoding.UTF8.GetString(stringBytes, 0, stringLength);
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            return BitConverter.ToSingle(ReceiveNetworkOrder(4, token), 0);
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            return BitConverter.ToUInt32(ReceiveNetworkOrder(4, token), 0);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        /// <summary>
        ///     Receives <paramref name="count" /> bytes, then shuffles them to host order.
        /// </summary>
        /// <param name="count">The number of bytes to receive</param>
        /// <param name="token">The token on which this receive operation can be cancelled.</param>
        /// <returns>The receiving buffer (in host order).</returns>
        private byte[] ReceiveNetworkOrder(int count, CancellationToken token)
        {
            var buf = _reader.ReadBytes(count);
            if (buf.Length != count)
            {
                // Stream has ended, which generally means we're about to be cancelled.
                token.WaitHandle.WaitOne();
            }
            token.ThrowIfCancellationRequested();
            BitManipulation.ShuffleForNetworkOrder(buf, count);
            return buf;
        }
    }
}