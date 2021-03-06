using System;
using System.Net.Sockets;
using System.Threading;
using URY.BAPS.Common.Infrastructure.Login;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.MessageIo;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Server.Protocol.V2.Io
{
    /// <summary>
    ///     Co-ordinates the server side of a BAPS client login using the V2 protocol.
    /// </summary>
    public class ServerSideLoginPerformer : ILoginPerformer<ClientHandle>
    {
        private readonly Func<IPrimitiveSource, CancellationToken, CommandDecoder> _decoderFunc;

        public ServerSideLoginPerformer(Func<IPrimitiveSource, CancellationToken, CommandDecoder> decoderFunc)
        {
            _decoderFunc = decoderFunc;
        }
        
        public bool TryLogin(TcpClient client, out ClientHandle handle)
        {
            IPrimitiveConnection? primConn = null;
            IMessageConnection? msgConn = null;
            
            try
            {
                primConn = new TcpPrimitiveConnection(client);
                // TODO(@MattWindsor91): handshake etc.
                msgConn = new MessageConnection(primConn, _decoderFunc);
                handle = new ClientHandle(msgConn);
                
                // Prevent disposals in finally block below.
                primConn = null;
                msgConn = null;
                return true;
            }
            finally
            {
                msgConn?.Dispose();
                primConn?.Dispose();
                client?.Close();
            }
        }
    }
}