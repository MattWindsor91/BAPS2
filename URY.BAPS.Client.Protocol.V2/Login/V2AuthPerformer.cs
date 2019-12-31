using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using URY.BAPS.Client.Common.Login;
using URY.BAPS.Client.Common.Login.LoginResult;
using URY.BAPS.Client.Common.Login.Prompt;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.MessageIo;
using URY.BAPS.Common.Protocol.V2.Ops;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

namespace URY.BAPS.Client.Protocol.V2.Login
{
    public sealed class V2AuthPerformer : IAuthPerformer<SeededPrimitiveConnection, IMessageConnection>, IDisposable
    {
        private readonly Func<IPrimitiveSource, CancellationToken, CommandDecoder> _commandDecoderFactory;
        public ILoginResult Result { get; private set; } = new SuccessLoginResult();
        public IMessageConnection Connection { get; private set; } = new DeadMessageConnection();

        public V2AuthPerformer(Func<IPrimitiveSource, CancellationToken, CommandDecoder> commandDecoderFactory)
        {
            _commandDecoderFactory = commandDecoderFactory;
        }
        
        public void Attempt(SeededPrimitiveConnection? conn, IAuthPromptResponse response)
        {
            try
            {
                if (conn is null) throw new ArgumentNullException(nameof(conn));
                if (response is null) throw new ArgumentNullException(nameof(response));
            
                Result = TryLogin(conn, response);
                if (!Result.IsSuccess) return;
                
                Connection = MakeMessageConnection(conn.Connection);
                // Stop the connection from being disposed by the 'finally' block if we successfully authenticated.
                conn = null;
            }
            finally
            {
                conn?.Dispose();
            }
        }

        private IMessageConnection MakeMessageConnection(IPrimitiveConnection conn)
        {
            return new MessageConnection(conn, _commandDecoderFactory);
        }

        private static ILoginResult TryLogin(SeededPrimitiveConnection seedConn, IAuthPromptResponse authPrompt)
        {
            var (seed, conn) = seedConn;
            
            var username = authPrompt.Username;
            var password = authPrompt.Password;
            var securedPassword = Md5Sum(string.Concat(seed, Md5Sum(password)));
 
            var loginCmd = new SystemCommand(SystemOp.Login);
            var loginMessage = new MessageBuilder(loginCmd).Add(username).Add(securedPassword);
            loginMessage.Send(conn);
 
            var (matched, authResult, description) = conn.ReceiveSystemStringCommand(SystemOp.LoginResult);
            if (!matched) return new InvalidProtocolLoginResult("login result listen");
 
            var authenticated = CommandUnpacking.Value(authResult) == 0;
            if (!authenticated) return new UserFailureLoginResult(description ?? "(no description)");
            return new SuccessLoginResult();
         }
 
         /** Generate an md5 sum of the raw argument **/
         private static string Md5Sum(string raw)
         {
             using var md5 = MD5.Create();
             var stringBuilder = new StringBuilder();
             var buffer = Encoding.ASCII.GetBytes(raw);
             var hash = md5.ComputeHash(buffer);
 
             foreach (var h in hash) stringBuilder.Append(h.ToString("x2", CultureInfo.InvariantCulture));
             return stringBuilder.ToString();
         }   
        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
    
    public static class AuthExtensions
     {
         public static (bool matched, ushort command, string? payload) ReceiveSystemStringCommand(
             this IPrimitiveSource src, SystemOp expectedOp)
         {
             var cmd = src.ReceiveCommand();
             _ = src.ReceiveUint(); // Discard length
             var isRightGroup = CommandUnpacking.Group(cmd) == CommandGroup.System;
             var isRightOp = CommandUnpacking.SystemOp(cmd) == expectedOp;
             var isRightCommand = isRightGroup && isRightOp;
             return isRightCommand ? (true, cmd, src.ReceiveString()) : (false, default, null);
         }
     }
}