namespace URY.BAPS.Client.Common.Login.LoginResult
{
    /// <summary>
    ///     Represents a login failure due to a mismatch in the way that the
    ///     BAPS client and server are talking to each other.
    /// </summary>
    public class InvalidProtocolLoginResult : LoginException
    {
        public InvalidProtocolLoginResult(string where) : base($"Invalid login protocol at {where}")
        {
        }

        public override bool IsFatal => true;
        public override bool IsUserFault => false;
    }
}