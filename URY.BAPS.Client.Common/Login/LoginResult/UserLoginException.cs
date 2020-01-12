namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Represents a login failure due to a user problem.
    /// </summary>
    public class UserLoginException : LoginException
    {
        public UserLoginException(string serverDescription) : base(serverDescription)
        {
        }

        public override bool IsFatal => false;

        public override bool IsUserFault => true;
    }
}