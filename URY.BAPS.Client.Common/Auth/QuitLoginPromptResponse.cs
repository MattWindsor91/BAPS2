namespace URY.BAPS.Client.Common.Auth
{
    /// <summary>
    ///     A <see cref="ILoginPromptResponse"/> that represents the situation where
    ///     the user gave up on trying to log into a server.
    /// </summary>
    public class QuitLoginPromptResponse : ILoginPromptResponse
    {
        public bool HasCredentials => false;
        public bool IsDifferentServer(ILoginPromptResponse other)
        {
            return other.HasCredentials;
        }

        public string Username => "";
        public string Password => "";
        public string Host => "";
        public int Port => -1;
    }
}