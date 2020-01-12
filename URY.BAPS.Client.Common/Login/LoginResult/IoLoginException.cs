using System;

namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Exception representing a 
    /// </summary>
    public class IoLoginException : LoginException
    {
        /// <summary>
        ///     Constructs a <see cref="IoLoginException"/> from a socket exception.
        /// </summary>
        /// <param name="e">
        ///     The exception whose error caused the login failure.
        /// </param>
        public IoLoginException(Exception e) : base("I/O failure", e)
        {
        }

        public override bool IsFatal => true;

        public override bool IsUserFault => false;
    }
}