using System;
using System.Collections.Generic;
using System.Text;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Server.Io
{
    /// <summary>
    ///     A handle for a particular BAPS server client.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly TcpConnection _connection;


        public Client(TcpConnection connection)
        {
            _connection = connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
