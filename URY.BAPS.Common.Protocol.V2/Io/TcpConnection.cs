using System;
using System.Net.Sockets;
using System.Threading;
using URY.BAPS.Common.Protocol.V2.Commands;

namespace URY.BAPS.Common.Protocol.V2.Io
{
    /// <summary>
    ///     This class defines all the low level network connection functions and functions
    ///     for how to send and receive the 4 fundamental data types used in BAPSNet.
    /// </summary>
    public class TcpConnection : IConnection, IDisposable
    {
        private readonly StreamPrimitiveSource _primitiveSource;

        /// <summary>
        ///     The low level socket connection
        /// </summary>
        private readonly TcpClient _clientSocket;

        private readonly StreamPrimitiveSink _primitiveSink;

        public TcpConnection(string host, int port)
        {
            _clientSocket = new TcpClient(host, port) {LingerState = new LingerOption(false, 0), NoDelay = true};
            var stream = _clientSocket.GetStream();

            _primitiveSink = new StreamPrimitiveSink(stream);
            _primitiveSource = new StreamPrimitiveSource(stream);
        }

        /// <summary>
        ///     Check if the socket is valid/connected.
        /// </summary>
        public bool IsValid => _clientSocket != null && _clientSocket.Connected;

        public ushort ReceiveCommand(CancellationToken token = default)
        {
            return _primitiveSource.ReceiveCommand(token);
        }

        public string ReceiveString(CancellationToken token = default)
        {
            return _primitiveSource.ReceiveString(token);
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            return _primitiveSource.ReceiveFloat(token);
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            return _primitiveSource.ReceiveUint(token);
        }

        public void SendCommand(ushort cmd)
        {
            _primitiveSink.SendCommand(cmd);
        }

        public void SendString(string s)
        {
            _primitiveSink.SendString(s);
        }

        public void SendFloat(float f)
        {
            _primitiveSink.SendFloat(f);
        }

        public void SendUint(uint i)
        {
            _primitiveSink.SendUint(i);
        }

        public void Flush()
        {
            _primitiveSink.Flush();
        }

        public void Dispose()
        {
            if (!IsValid) return;
            _primitiveSink.Dispose();
            _primitiveSource.Dispose();
            _clientSocket.Close();
        }
    }
}