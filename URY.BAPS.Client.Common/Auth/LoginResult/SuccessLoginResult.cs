namespace URY.BAPS.Client.Common.Auth.LoginResult
{
    /// <summary>
    ///     A successful login result.
    /// </summary>
    public class SuccessLoginResult : ILoginResult
    {
        public bool IsSuccess => true;
        public bool IsFatal => true;
        public bool IsUserFault => false;

        public string Description => "(success)";
    }
}
