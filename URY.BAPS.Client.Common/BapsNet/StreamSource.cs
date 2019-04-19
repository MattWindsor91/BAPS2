using System;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     Takes requests to receive BapsNet primitives, and applies them to a
    ///     <see cref="Stream"/> of bytes.
    /// </summary>
    public class StreamSource : ISource
    {
        private const int MaxReceiveBuffer = 512;

        /// <summary>
        ///     The receive buffer.
        /// </summary>
        private readonly byte[] _rxBytes = new byte[MaxReceiveBuffer];

        [NotNull] private readonly Stream _stream;

        public StreamSource([CanBeNull] Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <summary>
        ///     Generic receive function (limit MAX_RECEIVE_BUFFER), returned data is at start of byte array
        /// </summary>
        private byte[] Receive(int count, CancellationToken token)
        {
            if (MaxReceiveBuffer < count) throw new ArgumentOutOfRangeException(nameof(count));

            int nRead;
            for (var offset = 0; offset < count; offset += nRead)
            {
                token.ThrowIfCancellationRequested();
                nRead = _stream.Read(_rxBytes, offset, count - offset);
            }

            // TODO(@MattWindsor91): use a Span once we move to netcore
            return _rxBytes;
        }


        /// <summary>
        ///     As <see cref="Receive" />, but shuffles bytes to host order.
        /// </summary>
        /// <param name="count">The number of bytes to receive</param>
        /// <param name="token">The token on which this receive operation can be cancelled.</param>
        /// <returns>The receiving buffer (in host order).</returns>
        private byte[] ReceiveNetworkOrder(int count, CancellationToken token)
        {
            var buf = Receive(count, token);
            BitManipulation.ShuffleForNetworkOrder(buf, count);
            return buf;
        }

        public CommandWord ReceiveCommand(CancellationToken token = default)
        {
            return (CommandWord)BitConverter.ToUInt16(ReceiveNetworkOrder(2, token), 0);
        }

        public string ReceiveString(CancellationToken token = default)
        {
            /** As before we find out how long the string is first and then grab the UTF8 data **/
            var stringLength = (int)ReceiveUint(token);
            var sb = new StringBuilder(MaxReceiveBuffer);
            while (0 < stringLength)
            {
                var toReceive = Math.Min(stringLength, MaxReceiveBuffer);
                sb.Append(Encoding.UTF8.GetString(Receive(toReceive, token), 0, toReceive));
                stringLength -= toReceive;
            }
            return sb.ToString();
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            return BitConverter.ToSingle(ReceiveNetworkOrder(4, token), 0);
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            return BitConverter.ToUInt32(ReceiveNetworkOrder(4, token), 0);
        }
    }
}