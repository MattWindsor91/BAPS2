using System;
using System.Net.Sockets;
using URY.BAPS.Client.Common.Login.LoginResult;
using URY.BAPS.Client.Common.Login.Prompt;
using URY.BAPS.Common.Infrastructure.Login;

namespace URY.BAPS.Client.Common.Login
{
    /// <summary>
    ///     Co-ordinates the whole process of 'logging
    ///     into' a BAPS server.
    ///     <para>
    ///         The login performer takes a <see cref="TcpClient"/>
    ///         representing a newly-opened server connection.  It then
    ///         uses a <see cref="IHandshakePerformer{T}"/> to handle any
    ///         initial communications with the server before authentication.
    ///         It delegates authentication to three other objects: a
    ///         <see cref="IAuthPrompter"/> to pop up a dialog or other
    ///         interaction with the user; a <see cref="ILoginErrorHandler"/>
    ///         to communicate any errors to the user, and an
    ///         <see cref="IAuthPerformer{TRawConn,TAuthConn}"/> to perform
    ///         the final step of sending credentials to the server,
    ///         as well as any final connection bring-up necessary.
    ///     </para>
    ///     <para>
    ///         While the login performer itself is protocol-agnostic, the
    ///         handshake and authentication sub-objects are.
    ///     </para>
    /// </summary>
    /// <typeparam name="TRawConn">
    ///     Type of intermediate (often primitive-level) connections.
    /// </typeparam>
    /// <typeparam name="TAuthConn">
    ///     Type of authenticated (often message-level) connections.
    /// </typeparam>
    public class ClientSideLoginPerformer<TRawConn, TAuthConn> : ILoginPerformer<TAuthConn>
    {
        private readonly IHandshakePerformer<TRawConn> _handshake;
        private readonly IAuthPrompter _prompter;
        private readonly ILoginErrorHandler _errorHandler;
        private readonly IAuthPerformer<TRawConn, TAuthConn> _auth;

        /// <summary>
        ///     Constructs a <see cref="ClientSideLoginPerformer{TRawConn,TAuthConn}" />.
        /// </summary>
        /// <param name="handshake">
        ///     Object used to handle pre-authentication handshaking.
        /// </param>
        /// <param name="prompter">
        ///     Object used to handle login prompts.
        /// </param>
        /// <param name="errorHandler">
        ///     Object used to handle login errors.
        /// </param>
        /// <param name="auth">
        ///     Stateful object used to run login attempts.
        /// </param>
        /// <typeparam name="TAuthConn">
        ///     Type of authenticated connections.
        /// </typeparam>
        public ClientSideLoginPerformer(
            IHandshakePerformer<TRawConn>? handshake,
            IAuthPrompter? prompter, ILoginErrorHandler? errorHandler, IAuthPerformer<TRawConn, TAuthConn>? auth)
        {
            _handshake = handshake ?? throw new ArgumentNullException(nameof(handshake));
            _prompter = prompter ?? throw new ArgumentNullException(nameof(prompter));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        /// <summary>
        ///     The most recently authenticated connection.
        /// </summary>
        public TAuthConn Connection => _auth.Connection;

        /// <summary>
        ///     Whether the last call to <see cref="Run"/> was successful.
        /// </summary>
        public bool HasConnection { get; private set; }
        
        /// <summary>
        ///     Tries to construct an authenticated <see cref="TAuthConn" />.  If successful,
        ///     <see cref="HasConnection"/> will be <c>true</c>, and <see cref="Connection"/> will point to the
        ///     authenticated connection.
        /// </summary>
        public void Run(TcpClient client)
        {
            HasConnection = false;
            
            var result = _handshake.DoHandshake(client);
            if (result.IsSuccess)
            {
                LoginLoop(_handshake.Result);
            }
            else
            {
                HandleError(result);
            }
        }
        
        private void LoginLoop(TRawConn conn)
        {
            var shouldQuit = false;
            while (!shouldQuit) shouldQuit = AttemptAndHandleErrors(conn);
        }
        
        private bool AttemptAndHandleErrors(TRawConn conn)
        {
            var result = Attempt(conn);
            
            HasConnection = result.IsSuccess;
            if (!HasConnection) HandleError(result);
            
            return result.IsFatal;
        }

        private ILoginResult Attempt(TRawConn conn)
        {
            _prompter.Prompt();
            
            var response = _prompter.Response;
            if (!response.HasCredentials) return new QuitLoginResult();
            
            _auth.Attempt(conn, response);
            return _auth.Result;
        }

        private void HandleError(ILoginResult result)
        {
            _errorHandler.Handle(result);
        }

    }
}