using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace BAPSCommon
{
    /// <summary>
    /// Represents an attempt to create an authenticated BAPSnet connection.
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

        private bool done = false;
        private string lastServer = null;
        private int lastPort = -1;
        private string seed = null;
        private ClientSocket sock = null;

        /// <summary>
        /// Tries to construct an authenticated <see cref="ClientSocket"/>.
        /// </summary>
        /// <returns>Null, if we gave up trying to log in; a connected and ready client socket, otherwise.</returns>
        public ClientSocket Run()
        {
            while (!done) Attempt();
            return sock; // may be null if we gave up
        }

        private void DisposeSocketIfExists()
        {
            sock?.Dispose();
            sock = null;
        }

        private bool NeedNewConnection(Response response)
        {
            if (!ConnectionReady) return true;
            return lastServer != response.Server || lastPort != response.Port;
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
                sock = new ClientSocket(server, port, _token, _token);
            } catch (SocketException e)
            {
                /** If an error occurs just give the exception message and start again **/
                var errorMessage = $"System Error:\n{e.Message}\nStack Trace:\n{e.StackTrace}";
                ServerError?.Invoke(this, errorMessage);
                sock = null;
                return;
            }

            /** Receive the greeting string, this is the only communication
                that does not follow the 'command' 'command-length' 'argument1'...
                structure
             **/
            var greeting = sock.ReceiveS();
            var binaryModeCmd = new Message(Command.SYSTEM | Command.SETBINARYMODE);
            binaryModeCmd.Send(sock);
            /** Receive what should be the SEED command **/
            var seedCmd = sock.ReceiveC();
            /** Receive the length of the seed command **/
            sock.ReceiveI();
            /** Verify the server is sending what we expect **/
            if ((seedCmd & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.SEED))
            {
                IncompatibleLoginProcedure();
                return;
            }

            seed = sock.ReceiveS();
            Debug.Assert(seed != null, "Got a null seed despite making a connection");
        }

        private void IncompatibleLoginProcedure()
        {
            ServerError?.Invoke(this, "Server login procedure is not compatible with this client.");
            // Force a re-connect next time.
            DisposeSocketIfExists();
            seed = null;
        }

        private bool ConnectionReady => sock != null && seed != null;

        private void Attempt()
        {
            var result = _loginCallback();
            if (result.IsGivingUp)
            {
                DisposeSocketIfExists();
                done = true;
                return;
            }

            Debug.Assert(!done, "should only be done if we gave up or a previous attempt succeeded");

            MakeNewConnectionIfNeeded(result);
            lastServer = result.Server;
            lastPort = result.Port;

            if (!ConnectionReady) return;
            TryLogin(result.Username, result.Password);
        }

        /** Generate an md5 sum of the raw argument **/
        private string Md5sum(string raw)
        {
            var md5serv = System.Security.Cryptography.MD5.Create();
            var stringbuff = new System.Text.StringBuilder();
            var buffer = System.Text.Encoding.ASCII.GetBytes(raw);
            var hash = md5serv.ComputeHash(buffer);

            foreach (var h in hash)
            {
                stringbuff.Append(h.ToString("x2"));
            }
            return stringbuff.ToString();
        }

        private void TryLogin(string username, string password)
        {
            Debug.Assert(ConnectionReady, "tried to login without a waiting connection");
            var securedPassword = Md5sum(string.Concat(seed, Md5sum(password)));

            var loginCmd = new Message(Command.SYSTEM | Command.LOGIN).Add(username).Add(securedPassword);
            loginCmd.Send(sock);

            var authResult = sock.ReceiveC();
            /** Verify it is what we expected **/
            if ((authResult & (Command.GROUPMASK | Command.SYSTEM_OPMASK)) != (Command.SYSTEM | Command.LOGINRESULT))
            {
                IncompatibleLoginProcedure();
                return;
            }

            /** Receive the result command length **/
            sock.ReceiveI();
            /** Receive the description of the result code **/
            var description = sock.ReceiveS();
            /** Check the value for '0' meaning success **/
            var authenticated = (authResult & Command.SYSTEM_VALUEMASK) == 0;
            if (!authenticated)
            {
                UserError?.Invoke(this, description);
                return;
            }

            Debug.Assert(ConnectionReady, "was about to hand over a non-ready connection");
            done = true;
            return;
        }
    }
}
