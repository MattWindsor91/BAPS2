using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.BapsNet;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Io;
using URY.BAPS.Protocol.V2.Messages;

namespace URY.BAPS.Client.Common
{
    /// <summary>
    ///     Represents an attempt to create an authenticated BAPSNet connection.
    ///     <para>
    ///         Objects of this class are not thread-safe; only call
    ///         <see cref="Run" /> in one thread.
    ///     </para>
    /// </summary>
    public class Authenticator
    {
        private readonly Func<Response> _loginCallback;

        private int _lastPort = -1;
        private string? _lastServer;
        private string? _seed;
        private TcpConnection? _conn;

        /// <summary>
        ///     Constructs a <see cref="Authenticator" />.
        /// </summary>
        /// <param name="loginCallback">A callback that will be executed every time the authenticator needs data.</param>
        public Authenticator(Func<Response> loginCallback)
        {
            _loginCallback = loginCallback;
        }

        private bool ConnectionReady => _conn != null && _seed != null;

        /// <summary>
        ///     Raised with a description when a server error occurs.
        /// </summary>
        public event EventHandler<string> ServerError;

        /// <summary>
        ///     Raised with a description when a user error occurs.
        /// </summary>
        public event EventHandler<string> UserError;

        /// <summary>
        ///     Tries to construct an authenticated <see cref="TcpConnection" />.
        /// </summary>
        /// <returns>Null, if we gave up trying to log in; a connected and ready client socket, otherwise.</returns>
        public TcpConnection? Run()
        {
            var done = false;
            while (!done) done = Attempt();
            return _conn; // may be null if we gave up
        }

        private void DisposeSocketIfExists()
        {
            _conn?.Dispose();
            _conn = null;
        }

        [Pure]
        private bool NeedNewConnection(Response response)
        {
            if (!ConnectionReady) return true;
            return _lastServer != response.Server || _lastPort != response.Port;
        }

        private void MakeNewConnectionIfNeeded(Response response)
        {
            if (!NeedNewConnection(response)) return;
            DisposeSocketIfExists();

            MakeNewConnection(response.Server, response.Port);
        }

        private (CommandWord command, string? payload) ReceiveSystemStringCommand(SystemOp expectedOp, ISource src)
        {
            var cmd = src.ReceiveCommand();
            _ = src.ReceiveUint(); // Discard length
            var isRightGroup = cmd.Group() == CommandGroup.System;
            var isRightOp = cmd.SystemOp() == expectedOp;
            var isRightCommand = isRightGroup && isRightOp;
            if (isRightCommand) return (cmd, src.ReceiveString());
            IncompatibleLoginProcedure();
            return (default, null);
        }

        private void MakeNewConnection(string server, int port)
        {
            try
            {
                _conn = new TcpConnection(server, port);
            }
            catch (SocketException e)
            {
                /** If an error occurs just give the exception message and start again **/
                var errorMessage = $"System Error:\n{e.Message}\nStack Trace:\n{e.StackTrace}";
                ServerError?.Invoke(this, errorMessage);
                _conn = null;
                return;
            }

            /** Receive the greeting string, this is the only communication
                that does not follow the 'command' 'command-length' 'argument1'...
                structure
             **/
            _ = _conn.ReceiveString();
            var binaryModeCmd = new Message(CommandWord.System | CommandWord.SetBinaryMode);
            binaryModeCmd.Send(_conn);

            _seed = ReceiveSystemStringCommand(SystemOp.Seed, _conn).payload;
            Debug.Assert(_seed != null, "Got a null seed despite making a connection");
        }

        private void IncompatibleLoginProcedure()
        {
            ServerError?.Invoke(this, "Server login procedure is not compatible with this client.");
            // Force a re-connect next time.
            DisposeSocketIfExists();
            _seed = null;
        }

        private bool Attempt()
        {
            var result = _loginCallback();
            if (result.IsGivingUp)
            {
                DisposeSocketIfExists();
                return true;
            }

            MakeNewConnectionIfNeeded(result);
            _lastServer = result.Server;
            _lastPort = result.Port;

            return ConnectionReady && TryLogin(result.Username, result.Password);
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

        private bool TryLogin(string username, string password)
        {
            Debug.Assert(ConnectionReady, "tried to login without a waiting connection");
            if (_conn == null) return false;

            var securedPassword = Md5Sum(string.Concat(_seed, Md5Sum(password)));

            var loginCmd = new Message(CommandWord.System | CommandWord.Login).Add(username).Add(securedPassword);
            loginCmd.Send(_conn);

            var (authResult, description) = ReceiveSystemStringCommand(SystemOp.LoginResult, _conn);
            var authenticated = authResult.Value() == 0;
            if (!authenticated)
            {
                UserError?.Invoke(this, description ?? "(no description)");
                return false;
            }

            Debug.Assert(ConnectionReady, "was about to hand over a non-ready connection");
            return true;
        }

        /// <summary>
        ///     Type of responses that login callbacks should give.
        /// </summary>
        public struct Response
        {
            /// <summary>
            ///     If true, the login attempt has been abandoned, and all other
            ///     fields are undefined.
            /// </summary>
            public bool IsGivingUp;

            /// <summary>
            ///     The server hostname to connect to.
            /// </summary>
            public string Server;

            /// <summary>
            ///     The server TCP port to connect to.
            /// </summary>
            public int Port;

            /// <summary>
            ///     The username to connect with.
            /// </summary>
            public string Username;

            /// <summary>
            ///     The (unencrypted) password to connect with.
            /// </summary>
            public string Password;
        }
    }
}