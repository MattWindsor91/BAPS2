namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Represents a login failure due to a user problem.
    /// </summary>
    public class UserFailureLoginResult : ILoginResult
    {
        public UserFailureLoginResult(string serverDescription)
        {
            Description = serverDescription;
        }

        public bool IsSuccess => false;

        public bool IsFatal => false;

        public bool IsUserFault => true;

        public string Description { get; }
    }
}