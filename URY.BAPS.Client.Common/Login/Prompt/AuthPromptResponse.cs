namespace URY.BAPS.Client.Common.Login.Prompt
{
    public class AuthPromptResponse : IAuthPromptResponse
    {
        public AuthPromptResponse(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public bool HasCredentials => true;

        public string Username { get; }
        public string Password { get; }
    }
}
