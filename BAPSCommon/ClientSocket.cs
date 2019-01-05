using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BAPSCommon
{
    /// <summary>
	/// This class defines all the low level network connection functions and functions
	/// for how to send and receive the 4 fundamental data types used in BAPSNet.
    /// </summary>
    public class ClientSocket : IDisposable
    {
        private CancellationToken send_tok;
        private CancellationToken recv_tok;

        public ClientSocket(string host, int port, CancellationToken send_tok = default, CancellationToken recv_tok = default)
        {
            this.send_tok = send_tok;
            this.recv_tok = recv_tok;

            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(host), port));
            /** All sockets are non blocking due to lack of preemption on windows **/
            clientSocket.Blocking = false;
            /** Sockets will close without delay **/
            clientSocket.SetSocketOption(SocketOptionLevel.Tcp,
                                         SocketOptionName.NoDelay,
                                         1);
            /** Sockets shall linger for 0 milliseconds **/
            clientSocket.SetSocketOption(SocketOptionLevel.Socket,
                                         SocketOptionName.Linger,
                                         new LingerOption(false, 0));
        }

        public void Dispose()
        {
            if (!IsValid) return;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        #region Strings

        public void Send(string s)
        {
            /** Strings are a combination of integer length and then UTF8 data **/
            Send((uint)Encoding.UTF8.GetByteCount(s));
            Send(Encoding.UTF8.GetBytes(s));
        }
        public string ReceiveS()
        {
            /** As before we find out how long the string is first and then grab the UTF8 data **/
            var stringLength = (int)ReceiveI();
            var sb = new StringBuilder(MAX_RECEIVE_BUFFER);
            while (stringLength > 0)
            {
                var toReceive = Math.Min(stringLength, MAX_RECEIVE_BUFFER);
                sb.Append(Encoding.UTF8.GetString(Receive(toReceive), 0, toReceive));
                stringLength -= toReceive;
            }
            return sb.ToString();
        }

        #endregion Strings

        #region Commands

        public void Send(Command cmd) => SendN(BitConverter.GetBytes((ushort)cmd));
        public Command ReceiveC() => (Command)BitConverter.ToUInt16(ReceiveN(2), 0);

        #endregion Commands

        #region Floats

        public void Send(float f) => SendN(BitConverter.GetBytes(f));
        public float ReceiveF() => BitConverter.ToSingle(ReceiveN(4), 0);

        #endregion Floats

        #region Uints

        public void Send(uint i) => SendN(BitConverter.GetBytes(i));
        public uint ReceiveI() => BitConverter.ToUInt32(ReceiveN(4), 0);
        
        #endregion Uints

        /// <summary>
        /// Check if the socket is valid/connected.
        /// </summary>
        public bool IsValid => clientSocket != null && clientSocket.Connected;

        /// <summary>
        /// As <see cref="Send(byte[])"/>, but shuffles bytes to network order.
        /// </summary>
        /// <param name="bytes">The bytes to send (in host order).</param>
        private void SendN(byte[] bytes)
        {
            ShuffleForNetworkOrder(bytes, bytes.Length);
            Send(bytes);
        }

        /// <summary>
        /// As <see cref="Receive(int)"/>, but shuffles bytes to host order.
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
        /// Shuffles the first <paramref name="count"/> bytes of <paramref name="buffer"/>
        /// from host order to network order (or back).
        /// </summary>
        /// <param name="buffer">The buffer to modify in-place.</param>
        /// <param name="count">The number of bytes to shuffle.</param>
        private void ShuffleForNetworkOrder(byte[] buffer, int count)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer, 0, count);
        }

        /// <summary>
        /// Generic send function to reduce code duplication, simply sends all data in 'bytes'
        /// </summary>
        private void Send(byte[] bytes)
        {
            int nsent = 0;
            /** If sending no data then return immediately, else wait until it is all sent **/
            for (int index = 0; index < bytes.Length; index += nsent)
            {
                send_tok.ThrowIfCancellationRequested();

                /**
                 *  Manage exceptions and maintain a count of the bytes sent and therefore the position
                 *  in the byte array
                **/
                try
                {
                    nsent = clientSocket.Send(bytes, index, bytes.Length - index, SocketFlags.None);
                }
                catch (SocketException e)
                {
                    /** Connection error must have occurred, rethrow exception **/
                    if (e.SocketErrorCode != SocketError.WouldBlock) throw e;
                    /** We are flooding the connection this is bad
                     *  WORK NEEDED: possible implementation of lossy sends for non vital data
                    **/
                    Thread.Sleep(1);
                }
            }
        }

        private const int MAX_RECEIVE_BUFFER = 512;

        /// <summary>
        /// Generic receive function (limit MAX_RECEIVE_BUFFER), returned data is at start of byte array
        /// </summary>
        private byte[] Receive(int count)
        {
            if (MAX_RECEIVE_BUFFER < count) throw new ArgumentOutOfRangeException("count");

            int nread = 0;
            for (int offset = 0; offset < count; offset += nread)
            {
                recv_tok.ThrowIfCancellationRequested();

                try
                {
                    nread = clientSocket.Receive(rxBytes, offset, count - offset, SocketFlags.None);
                } catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.WouldBlock) throw e;
                }
            }

            // TODO(@MattWindsor91): use a Span once we move to netcore
            return rxBytes;
        }

        /// <summary>
        /// The low level socket connection
        /// </summary>
        private Socket clientSocket =
            new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      ProtocolType.Tcp);
        /// <summary>
        /// The receive buffer
        /// </summary>
		private readonly byte[] rxBytes = new byte[MAX_RECEIVE_BUFFER];

        public void ShutdownReceive()
        {
            if (!IsValid) return;
            clientSocket.Shutdown(SocketShutdown.Receive);
        }

        public void ShutdownSend()
        {
            if (!IsValid) return;
            clientSocket.Shutdown(SocketShutdown.Send);
        }
    }
}
