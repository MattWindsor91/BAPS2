namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     Represents a login failure due to a mismatch in the way that the
    ///     BAPS client and server are talking to each other.
    /// </summary>
    public class InvalidProtocolLoginResult : ILoginResult
    {
        private readonly string _where;

        public InvalidProtocolLoginResult(string where)
        {
            _where = where;
        }

        public bool IsSuccess => false;
        public bool IsFatal => true;
        public bool IsUserFault => false;
        public string Description => $"Invalid login protocol at {_where}.";
    }
}