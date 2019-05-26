using System;

namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     Coordinates the creation of authenticated BAPS client connections.
    /// </summary>
    public class Authenticator<TConn> where TConn : class
    {
        private readonly ILoginPrompter _prompter;
        private readonly ILoginErrorHandler _errorHandler;
        private readonly IAuthedConnectionBuilder<TConn> _builder;


        /// <summary>
        ///     Constructs a <see cref="Authenticator" />.
        /// </summary>
        /// <param name="prompter">
        ///     Object used to handle login prompts.
        /// </param>
        /// <param name="errorHandler">
        ///     Object used to handle login errors.
        /// </param>
        /// <param name="builder">
        ///     Stateful object used to run login attempts.
        /// </param>
        /// <typeparam name="TConn">
        ///     Type of authenticated connections.
        /// </typeparam>
        public Authenticator(ILoginPrompter? prompter, ILoginErrorHandler? errorHandler, IAuthedConnectionBuilder<TConn>? builder)
        {
            _prompter = prompter ?? throw new ArgumentNullException(nameof(prompter));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }



        /// <summary>
        ///     Tries to construct an authenticated <see cref="TcpConnection" />.
        /// </summary>
        /// <returns>Null, if we gave up trying to log in; a connected and ready client socket, otherwise.</returns>
        public TConn? Run()
        {
            var done = false;
            while (!done) done = AttemptAndHandleErrors();
            return _builder.Connection;
        }

        private ILoginResult Attempt()
        {
            _prompter.Prompt();
            var response = _prompter.Response;
            if (!response.HasCredentials) return new SuccessLoginResult();
            return _builder.Attempt(response);
        }

        private bool AttemptAndHandleErrors()
        {
            var result = Attempt();
            if (!result.IsSuccess) HandleError(result);
            return result.IsDone;
        }

        private void HandleError(ILoginResult result)
        {
            _errorHandler.Handle(result);
        }

    }
}