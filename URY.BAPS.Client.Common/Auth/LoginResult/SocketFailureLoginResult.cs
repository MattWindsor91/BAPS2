using System;

namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     Login result representing a socket failure.
    /// </summary>
    public class SocketFailureLoginResult : ILoginResult
    {
        /// <summary>
        ///     Constructs a <see cref="SocketFailureLoginResult"/> from a socket exception.
        /// </summary>
        /// <param name="e">
        ///     The exception whose error caused the login failure.
        /// </param>
        public SocketFailureLoginResult(Exception e)
        {
            Description = e.Message;
        }

        public bool IsSuccess => false;

        public bool IsFatal => true;

        public bool IsUserFault => false;

        public string Description { get; }
    }
}