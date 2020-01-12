using System;
using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    ///     An <see cref="IAuthPerformer{TRawConn,TAuthConn}" /> that handles the authentication side of the construction
    ///     of a V2 BapsNet connection.
    /// </summary>
    public sealed class V2AuthPerformer : IAuthPerformer<SeededPrimitiveConnection, IMessageConnection>, IDisposable
    {
        private readonly Func<IPrimitiveSource, CancellationToken, CommandDecoder> _commandDecoderFactory;

        public V2AuthPerformer(Func<IPrimitiveSource, CancellationToken, CommandDecoder> commandDecoderFactory)
        {
            _commandDecoderFactory = commandDecoderFactory;
        }

        public ILoginResult Result { get; private set; } = new SuccessLoginResult();
        public IMessageConnection Connection { get; private set; } = new DeadMessageConnection();

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

        public void Dispose()
        {
            Connection?.Dispose();
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
            if (!authenticated) return new UserLoginException(description ?? "(no description)");
            return new SuccessLoginResult();
        }

        /// <summary>
        ///     Generates a MD5 sum of the raw argument.
        ///     <para>
        ///         MD5 is woefully insecure now, but it's been grandfathered in by the v2 BAPS protocol, and we have
        ///         to keep supporting it.
        ///     </para>
        /// </summary>
        /// <param name="raw">The raw string to hash.</param>
        /// <returns>A MD5 digest over <paramref name="raw" />.</returns>
        [SuppressMessage("Microsoft.Cryptography", "CA5351", Justification = "Required by the BAPS v2 protocol.")]
        private static string Md5Sum(string raw)
        {
            using var md5 = MD5.Create();
            var stringBuilder = new StringBuilder();
            var buffer = Encoding.ASCII.GetBytes(raw);
            var hash = md5.ComputeHash(buffer);

            foreach (var h in hash) stringBuilder.Append(h.ToString("x2", CultureInfo.InvariantCulture));
            return stringBuilder.ToString();
        }
    }
}