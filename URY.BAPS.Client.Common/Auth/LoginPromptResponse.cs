namespace URY.BAPS.Client.Common.Auth
{
    public class LoginPromptResponse : ILoginPromptResponse
    {
        public LoginPromptResponse(string username, string password, string host, int port)
        {
            Username = username;
            Password = password;
            Host = host;
            Port = port;
        }

        public bool HasCredentials => true;
        public bool IsDifferentServer(ILoginPromptResponse other)
        {
            if (!other.HasCredentials) return true;
            if (Host != other.Host) return true;
            return Port != other.Port;
        }

        public string Username { get; }
        public string Password { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
