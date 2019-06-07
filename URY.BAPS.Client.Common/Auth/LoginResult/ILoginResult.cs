namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     Interface for results returned from the inner parts of an
    ///     <see cref="Authenticator{TConn}"/> stack.
    /// </summary>
    public interface ILoginResult
    {
        /// <summary>
        ///     Whether the login succeeded.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        ///     Whether the login should be abandoned.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        ///     Whether the login failed due to a user error or decision.
        /// </summary>
        bool IsUserFault { get; }

        /// <summary>
        ///     Gets any error message associated with the outcome.
        /// </summary>
        string Description { get; }
    }
}