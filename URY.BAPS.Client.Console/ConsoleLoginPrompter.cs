using System;
using System.Text;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Console
{
    /// <summary>
    ///     A <see cref="ILoginPrompter"/> that uses the console.
    /// </summary>
    public class ConsoleLoginPrompter : ILoginPrompter
    {
        private readonly ClientConfig _config;

        /// <summary>
        ///     Creates a <see cref="ConsoleLoginPrompter"/>
        /// </summary>
        /// <param name="configManager">
        ///     The config manager, used to retrieve the default address,
        ///     port, and username.
        /// </param>
        public ConsoleLoginPrompter(IClientConfigManager configManager)
        {
            _config = configManager.LoadConfig();
        }

        public void Prompt()
        {
            var serverAddress = GetServerAddress();
            var serverPort = GetServerPort();
            var username = GetUsername();
            var password = GetPassword();

            Response = new LoginPromptResponse(username, password, serverAddress, serverPort);
        }

        private string GetServerAddress()
        {
            return GetText("Server address", _config.ServerAddress);
        }

        private int GetServerPort()
        {
            var portString = GetText("Server port", _config.ServerPort.ToString());
            return int.Parse(portString);
        }

        private string GetUsername()
        {
            return GetText("Username", _config.DefaultUsername);
        }

        private static string GetPassword()
        {
            System.Console.Write("Password: ");
            var sb = GetPasswordWithoutPrompt();
            System.Console.WriteLine();
            return sb;
        }

        private static string GetPasswordWithoutPrompt()
        {
            // TODO(@MattWindsor91): this isn't even remotely secure,
            // but it isn't clear what the alternatives are.
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                // See https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring
                // (we don't actually use SecureString as it's informally
                // deprecated).
                key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && 0 < sb.Length)
                {
                    sb.Remove(sb.Length - 1, 1);
                    System.Console.Write('\b');
                    continue;
                }

                var ch = key.KeyChar;
                if (char.IsControl(ch)) continue;

                sb.Append(ch);
                System.Console.Write('*');
            } while (key.Key != ConsoleKey.Enter);

            return sb.ToString();
        }

        private string GetText(string prompt, string defaultValue)
        {
            System.Console.Write($"{prompt} (default: {defaultValue}): ");
            var input = System.Console.ReadLine();
            return input == "" ? defaultValue : input;
        }

        public ILoginPromptResponse Response { get; private set; } = new QuitLoginPromptResponse();
    }
}
