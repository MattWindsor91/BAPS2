using System;
using System.Net.Sockets;
using System.Threading;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Common.Protocol.V2.MessageIo
{
    /// <summary>
    ///     A factory that builds message-IO connections over <see cref="TcpClient"/>s.
    /// </summary>
    public class ConnectionFactory<T> where T : class, IConnection
    {
        private readonly Func<TcpClient, PrimitiveIo.IConnection> _primitiveConnFactory;
        private readonly Func<IPrimitiveSource, CancellationToken, CommandDecoder> _decoderFactory;
        private readonly Func<Connection, T> _finalConnectionFactory;

        public ConnectionFactory(
            Func<TcpClient, PrimitiveIo.IConnection> primitiveConnFactory,
            Func<IPrimitiveSource, CancellationToken, CommandDecoder> decoderFactory,
            Func<Connection, T> finalConnectionFactory)
        {
            _primitiveConnFactory = primitiveConnFactory;
            _decoderFactory = decoderFactory;
            _finalConnectionFactory = finalConnectionFactory;
        }

        public T Build(TcpClient client)
        {
            PrimitiveIo.IConnection? pconn = null;
            Connection? mconn = null;
            try
            {
                pconn = _primitiveConnFactory(client);
                mconn = new Connection(pconn, _decoderFactory);
                return _finalConnectionFactory(mconn);
            }
            finally
            {
                mconn?.Dispose();
                pconn?.Dispose();
            }
        }
    }
}