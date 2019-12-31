using System.Net.Sockets;
using URY.BAPS.Client.Common.Login;
using URY.BAPS.Client.Common.Login.LoginResult;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Ops;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Client.Protocol.V2.Login
{
    /// <summary>
    ///     An object that performs the initial handshake portion of log-in
    ///     using version 2 of the BapsNet protocol.
    /// </summary>
    public class V2HandshakePerformer : IHandshakePerformer<SeededPrimitiveConnection>
    {
        private readonly ILoginResult _success = new SuccessLoginResult();
        private SeededPrimitiveConnection _result;

        public SeededPrimitiveConnection Result => _result;

        public ILoginResult DoHandshake(TcpClient client)
        {
            TcpPrimitiveConnection? connection = null;
            try
            {
                connection = new TcpPrimitiveConnection(client);

                /* Receive the greeting string, this is the only communication
                    that does not follow the 'command' 'command-length' 'argument1'...
                    structure
                 */
                _ = connection.ReceiveString();
                SetBinaryMode(connection);

                var (wasSeed, _, seed) = connection.ReceiveSystemStringCommand(SystemOp.Seed);
                if (!wasSeed) return new InvalidProtocolLoginResult("seed");
                _result = new SeededPrimitiveConnection(seed, connection);
                connection = null;
                return _success;
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private static void SetBinaryMode(IPrimitiveSink connection)
        {
            var binaryModeCommand = new SystemCommand(SystemOp.SetBinaryMode);
            var binaryModeMessage = new MessageBuilder(binaryModeCommand);
            binaryModeMessage.Send(connection);
        }
    }
}