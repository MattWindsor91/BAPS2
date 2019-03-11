using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using URY.BAPS.Client.Common.BapsNet;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     This class defines all the low level network connection functions and functions
    ///     for how to send and receive the 4 fundamental data types used in BAPSNet.
    /// </summary>
    public class ClientSocket : IDisposable, ISink
    {
        private const int MaxReceiveBuffer = 512;

        /// <summary>
        ///     The low level socket connection
        /// </summary>
        private readonly TcpClient _clientSocket;

        private readonly CancellationToken _receiveTok;

        /// <summary>
        ///     The receive buffer
        /// </summary>
        private readonly byte[] _rxBytes = new byte[MaxReceiveBuffer];

        private readonly CancellationToken _sendTok;

        public ClientSocket(string host, int port, CancellationToken sendTok = default,
            CancellationToken receiveTok = default)
        {
            _sendTok = sendTok;
            _receiveTok = receiveTok;

            _clientSocket = new TcpClient(host, port);
            _clientSocket.LingerState = new LingerOption(false, 0);
            _clientSocket.NoDelay = true;
        }

        /// <summary>
        ///     Check if the socket is valid/connected.
        /// </summary>
        public bool IsValid => _clientSocket != null && _clientSocket.Connected;

        public void Dispose()
        {
            if (!IsValid) return;
            _clientSocket.Close();
        }

        /// <summary>
        ///     As <see cref="Send(byte[])" />, but shuffles bytes to network order.
        /// </summary>
        /// <param name="bytes">The bytes to send (in host order).</param>
        private void SendN(byte[] bytes)
        {
            ShuffleForNetworkOrder(bytes, bytes.Length);
            Send(bytes);
        }

        /// <summary>
        ///     As <see cref="Receive(int)" />, but shuffles bytes to host order.
        /// </summary>
        /// <param name="count">The number of bytes to receive</param>
        /// <returns>The receiving buffer (in host order).</returns>
        private byte[] ReceiveN(int count)
        {
            var buf = Receive(count);
            ShuffleForNetworkOrder(buf, count);
            return buf;
        }


        /// <summary>
        ///     Shuffles the first <paramref name="count" /> bytes of <paramref name="buffer" />
        ///     from host order to network order (or back).
        /// </summary>
        /// <param name="buffer">The buffer to modify in-place.</param>
        /// <param name="count">The number of bytes to shuffle.</param>
        private static void ShuffleForNetworkOrder(byte[] buffer, int count)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer, 0, count);
        }

        /// <summary>
        ///     Generic send function to reduce code duplication, simply sends all data in 'bytes'
        /// </summary>
        private void Send(byte[] bytes)
        {
            var stream = _clientSocket.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///     Generic receive function (limit MAX_RECEIVE_BUFFER), returned data is at start of byte array
        /// </summary>
        private byte[] Receive(int count)
        {
            if (MaxReceiveBuffer < count) throw new ArgumentOutOfRangeException(nameof(count));

            var stream = _clientSocket.GetStream();

            int nRead = 0;
            for (var offset = 0; offset < count; offset += nRead)
            {
                _receiveTok.ThrowIfCancellationRequested();
                nRead = stream.Read(_rxBytes, offset, count - offset);
            }

            // TODO(@MattWindsor91): use a Span once we move to netcore
            return _rxBytes;
        }

        #region Strings

        public void SendString(string s)
        {
            /** Strings are a combination of integer length and then UTF8 data **/
            SendU32((uint) Encoding.UTF8.GetByteCount(s));
            Send(Encoding.UTF8.GetBytes(s));
        }

        public string ReceiveS()
        {
            /** As before we find out how long the string is first and then grab the UTF8 data **/
            var stringLength = (int) ReceiveI();
            var sb = new StringBuilder(MaxReceiveBuffer);
            while (stringLength > 0)
            {
                var toReceive = Math.Min(stringLength, MaxReceiveBuffer);
                sb.Append(Encoding.UTF8.GetString(Receive(toReceive), 0, toReceive));
                stringLength -= toReceive;
            }

            return sb.ToString();
        }

        #endregion Strings

        #region Commands

        public void SendCommand(Command cmd)
        {
            SendN(BitConverter.GetBytes((ushort) cmd));
        }

        public Command ReceiveC()
        {
            return (Command) BitConverter.ToUInt16(ReceiveN(2), 0);
        }

        #endregion Commands

        #region Floats

        public void SendFloat(float f)
        {
            SendN(BitConverter.GetBytes(f));
        }

        public float ReceiveF()
        {
            return BitConverter.ToSingle(ReceiveN(4), 0);
        }

        #endregion Floats

        #region Uints

        public void SendU32(uint i)
        {
            SendN(BitConverter.GetBytes(i));
        }

        public uint ReceiveI()
        {
            return BitConverter.ToUInt32(ReceiveN(4), 0);
        }

        #endregion Uints
    }
}