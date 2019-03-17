using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Client.Common.Utils;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     This class defines all the low level network connection functions and functions
    ///     for how to send and receive the 4 fundamental data types used in BAPSNet.
    /// </summary>
    public class TcpConnection : IDisposable
    {
        public ISink Sink { get; }
        public ISource Source { get; }

        /// <summary>
        ///     The low level socket connection
        /// </summary>
        private readonly TcpClient _clientSocket;

        public TcpConnection(string host, int port)
        {
            _clientSocket = new TcpClient(host, port) {LingerState = new LingerOption(false, 0), NoDelay = true};
            var stream = _clientSocket.GetStream();

            Sink = new StreamSink(stream);
            Source = new StreamSource(stream);
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
    }
}