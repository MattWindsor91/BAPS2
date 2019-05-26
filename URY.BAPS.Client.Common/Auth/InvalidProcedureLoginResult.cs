namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     Represents a login failure due to a mismatch in the way that the
    ///     BAPS client and server are talking to each other.
    /// </summary>
    public class InvalidProcedureLoginResult : ILoginResult
    {
        private readonly string _where;

        public InvalidProcedureLoginResult(string where)
        {
            _where = where;
        }

        public bool IsSuccess => false;
        public bool IsDone => false;
        public bool IsUserFault => false;
        public string Description => $"Invalid BAPS2 login procedure at {_where}.";
    }
}