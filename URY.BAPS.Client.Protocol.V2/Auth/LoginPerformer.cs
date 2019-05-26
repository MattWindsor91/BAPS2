using System.Security.Cryptography;
using System.Text;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Encode;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Protocol.V2.Auth
{
    public static class AuthExtensions
    {
        public static (bool matched, CommandWord command, string? payload) ReceiveSystemStringCommand(this IPrimitiveSource src, SystemOp expectedOp)
        {
            var cmd = src.ReceiveCommand();
            _ = src.ReceiveUint(); // Discard length
            var isRightGroup = cmd.Group() == CommandGroup.System;
            var isRightOp = cmd.SystemOp() == expectedOp;
            var isRightCommand = isRightGroup && isRightOp;
            return isRightCommand ? (true, cmd, src.ReceiveString()) : (false, default, null);
        }
    }

    public class LoginPerformer
    {
        private readonly TcpConnection _connection;
        private readonly string _seed;

        public LoginPerformer(TcpConnection connection, string seed)
        {
            _connection = connection;
            _seed = seed;
        }

        public ILoginResult TryLogin(ILoginPromptResponse loginPrompt)
        {
            var username = loginPrompt.Username;
            var password = loginPrompt.Password;
            var securedPassword = Md5Sum(string.Concat(_seed, Md5Sum(password)));

            var loginCmd = new SystemCommand(SystemOp.Login);
            var loginMessage = new MessageBuilder(loginCmd).Add(username).Add(securedPassword);
            loginMessage.Send(_connection);

            var (matched, authResult, description) = _connection.ReceiveSystemStringCommand(SystemOp.LoginResult);
            if (!matched) return new InvalidProcedureLoginResult("login result listen");

            var authenticated = authResult.Value() == 0;
            if (!authenticated) return new UserFailureLoginResult(description ?? "(no description)");
            return new SuccessLoginResult();
        }

        /** Generate an md5 sum of the raw argument **/
        private static string Md5Sum(string raw)
        {
            var md5 = MD5.Create();
            var stringBuilder = new StringBuilder();
            var buffer = Encoding.ASCII.GetBytes(raw);
            var hash = md5.ComputeHash(buffer);

            foreach (var h in hash) stringBuilder.Append(h.ToString("x2"));
            return stringBuilder.ToString();
        }
    }
}
