﻿using System;
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
        private readonly StreamBapsNetSource _bapsNetSource;

        /// <summary>
        ///     The low level socket connection
        /// </summary>
        private readonly TcpClient _clientSocket;

        private readonly StreamSink _sink;

        public TcpConnection(string host, int port)
        {
            _clientSocket = new TcpClient(host, port) {LingerState = new LingerOption(false, 0), NoDelay = true};
            var stream = _clientSocket.GetStream();

            _sink = new StreamSink(stream);
            _bapsNetSource = new StreamBapsNetSource(stream);
        }

        /// <summary>
        ///     Check if the socket is valid/connected.
        /// </summary>
        public bool IsValid => _clientSocket != null && _clientSocket.Connected;

        public CommandWord ReceiveCommand(CancellationToken token = default)
        {
            return _bapsNetSource.ReceiveCommand(token);
        }

        public string ReceiveString(CancellationToken token = default)
        {
            return _bapsNetSource.ReceiveString(token);
        }

        public float ReceiveFloat(CancellationToken token = default)
        {
            return _bapsNetSource.ReceiveFloat(token);
        }

        public uint ReceiveUint(CancellationToken token = default)
        {
            return _bapsNetSource.ReceiveUint(token);
        }

        public void SendCommand(CommandWord cmd)
        {
            _sink.SendCommand(cmd);
        }

        public void SendString(string s)
        {
            _sink.SendString(s);
        }

        public void SendFloat(float f)
        {
            _sink.SendFloat(f);
        }

        public void SendUint(uint i)
        {
            _sink.SendUint(i);
        }

        public void Flush()
        {
            _sink.Flush();
        }

        public void Dispose()
        {
            if (!IsValid) return;
            _sink.Dispose();
            _bapsNetSource.Dispose();
            _clientSocket.Close();
        }
    }
}