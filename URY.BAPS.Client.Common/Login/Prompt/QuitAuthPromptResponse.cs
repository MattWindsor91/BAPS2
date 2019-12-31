namespace URY.BAPS.Client.Common.Login.Prompt
{
    /// <summary>
    ///     A <see cref="IAuthPromptResponse"/> that represents the situation where
    ///     the user gave up on trying to log into a server.
    /// </summary>
    public class QuitAuthPromptResponse : IAuthPromptResponse
    {
        public bool HasCredentials => false;
        public bool IsDifferentServer(IAuthPromptResponse other)
        {
            return other.HasCredentials;
        }

        public string Username => "";
        public string Password => "";
    }
}