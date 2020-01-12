using System;

namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Base class of login exceptions.
    /// </summary>
    public abstract class LoginException : Exception
    {
        protected LoginException(string message) : base(message)
        {
        }

        protected LoginException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Whether the login should be abandoned.
        /// </summary>
        public abstract bool IsFatal { get; }

        /// <summary>
        ///     Whether the login failed due to a user error or decision.
        /// </summary>
        public abstract bool IsUserFault { get; }
    }
}