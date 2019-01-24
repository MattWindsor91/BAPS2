using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using BAPSClientCommon.BapsNet;

namespace BAPSClientCommon
{
    /// <summary>
    /// Represents an attempt to create an authenticated BAPSNet connection.
    /// <para>
    /// Objects of this class are not thread-safe; only call
    /// <see cref="Run"/> in one thread.
    /// </para>
    /// </summary>
    public class Authenticator
    {
        /// <summary>
        /// Type of responses that login callbacks should give.
        /// </summary>
        public struct Response
        {
            /// <summary>
            /// If true, the login attempt has been abandoned, and all other
            /// fields are undefined.
            /// </summary>
            public bool IsGivingUp;
            /// <summary>
            /// The server hostname to connect to.
            /// </summary>
            public string Server;
            /// <summary>
            /// The server TCP port to connect to.
            /// </summary>
            public int Port;
            /// <summary>
            /// The username to connect with.
            /// </summary>
            public string Username;
            /// <summary>
            /// The (unencrypted) password to connect with.
            /// </summary>
            public string Password;
        }

        /// <summary>
        /// Constructs a <see cref="Authenticator"/>.
        /// </summary>
        /// <param name="loginCallback">A callback that will be executed every time the authenticator needs data.</param>
        /// <param name="token">The cancellation token to pass to any constructed client sockets.</param>
        public Authenticator(Func<Response> loginCallback, CancellationToken token)
        {
            _loginCallback = loginCallback;
            _token = token;
        }

        /// <summary>
        /// Raised with a description when a server error occurs.
        /// </summary>
        public event EventHandler<string> ServerError;

        /// <summary>
        /// Raised with a description when a user error occurs.
        /// </summary>
        public event EventHandler<string> UserError;

        private readonly Func<Response> _loginCallback;
        private readonly CancellationToken _token;

        private bool _done;
        private string _lastServer;
        private int _lastPort = -1;
        private string _seed;
        private ClientSocket _sock;

        /// <summary>
        /// Tries to construct an authenticated <see cref="ClientSocket"/>.
        /// </summary>
        /// <returns>Null, if we gave up trying to log in; a connected and ready client socket, otherwise.</returns>
        public ClientSocket Run()
        {
            while (!_done) Attempt();
            return _sock; // may be null if we gave up
        }

        private void DisposeSocketIfExists()
        {
            _sock?.Dispose();
            _sock = null;
        }

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

        private void MakeNewConnection(string server, int port)
        {
            try
            {
                _sock = new ClientSocket(server, port, _token, _token);
            } catch (SocketException e)
            {
                /** If an error occurs just give the exception message and start again **/
                var errorMessage = $"System Error:\n{e.Message}\nStack Trace:\n{e.StackTrace}";
                ServerError?.Invoke(this, errorMessage);
                _sock = null;
                return;
            }

            /** Receive the greeting string, this is the only communication
                that does not follow the 'command' 'command-length' 'argument1'...
                structure
             **/
            _ = _sock.ReceiveS();
            var binaryModeCmd = new Message(Command.System | Command.SetBinaryMode);
            binaryModeCmd.Send(_sock);
            /** Receive what should be the SEED command **/
            var seedCmd = _sock.ReceiveC();
            /** Receive the length of the seed command **/
            _sock.ReceiveI();
            /** Verify the server is sending what we expect **/
            if ((seedCmd & (Command.GroupMask | Command.SystemOpMask)) != (Command.System | Command.Seed))
            {
                IncompatibleLoginProcedure();
                return;
            }

            _seed = _sock.ReceiveS();
            Debug.Assert(_seed != null, "Got a null seed despite making a connection");
        }

        private void IncompatibleLoginProcedure()
        {
            ServerError?.Invoke(this, "Server login procedure is not compatible with this client.");
            // Force a re-connect next time.
            DisposeSocketIfExists();
            _seed = null;
        }

        private bool ConnectionReady => _sock != null && _seed != null;

        private void Attempt()
        {
            var result = _loginCallback();
            if (result.IsGivingUp)
            {
                DisposeSocketIfExists();
                _done = true;
                return;
            }

            Debug.Assert(!_done, "should only be done if we gave up or a previous attempt succeeded");

            MakeNewConnectionIfNeeded(result);
            _lastServer = result.Server;
            _lastPort = result.Port;

            if (!ConnectionReady) return;
            TryLogin(result.Username, result.Password);
        }

        /** Generate an md5 sum of the raw argument **/
        private static string Md5Sum(string raw)
        {
            var md5Serv = MD5.Create();
            var stringbuff = new StringBuilder();
            var buffer = Encoding.ASCII.GetBytes(raw);
            var hash = md5Serv.ComputeHash(buffer);

            foreach (var h in hash)
            {
                stringbuff.Append(h.ToString("x2"));
            }
            return stringbuff.ToString();
        }

        private void TryLogin(string username, string password)
        {
            Debug.Assert(ConnectionReady, "tried to login without a waiting connection");
            var securedPassword = Md5Sum(string.Concat(_seed, Md5Sum(password)));

            var loginCmd = new Message(Command.System | Command.Login).Add(username).Add(securedPassword);
            loginCmd.Send(_sock);

            var authResult = _sock.ReceiveC();
            /** Verify it is what we expected **/
            if ((authResult & (Command.GroupMask | Command.SystemOpMask)) != (Command.System | Command.LoginResult))
            {
                IncompatibleLoginProcedure();
                return;
            }

            /** Receive the result command length **/
            _sock.ReceiveI();
            /** Receive the description of the result code **/
            var description = _sock.ReceiveS();
            /** Check the value for '0' meaning success **/
            var authenticated = (authResult & Command.SystemValueMask) == 0;
            if (!authenticated)
            {
                UserError?.Invoke(this, description);
                return;
            }

            Debug.Assert(ConnectionReady, "was about to hand over a non-ready connection");
            _done = true;
        }
    }
}
