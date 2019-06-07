namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     A successful login result.
    /// </summary>
    public class SuccessLoginResult : ILoginResult
    {
        public bool IsSuccess => true;
        public bool IsDone => true;
        public bool IsUserFault => false;

        public string Description => "(success)";
    }
}
