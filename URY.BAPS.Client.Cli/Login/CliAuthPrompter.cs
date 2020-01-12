using System;
using System.Text;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.Login.Prompt;
using static URY.BAPS.Client.Cli.CliUtilities;

namespace URY.BAPS.Client.Cli.Login
{
    /// <summary>
    ///     A <see cref="IAuthPrompter"/> that uses the console.
    /// </summary>
    public class CliAuthPrompter : IAuthPrompter
    {
        private readonly ClientConfig _config;

        /// <summary>
        ///     Creates a <see cref="CliAuthPrompter"/>
        /// </summary>
        /// <param name="config">
        ///     The client configuration used to retrieve the default username.
        /// </param>
        public CliAuthPrompter(ClientConfig config)
        {
            _config = config;
        }

        public void Prompt()
        {
            var username = GetUsername();
            var password = GetPassword();

            Response = new AuthPromptResponse(username, password);
        }

        private string GetUsername()
        {
            return GetText("Username", _config.DefaultUsername);
        }

        private static string GetPassword()
        {
            return ReadLine.ReadPassword("Password: ");
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
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && 0 < sb.Length)
                {
                    sb.Remove(sb.Length - 1, 1);
                    Console.Write('\b');
                    continue;
                }

                var ch = key.KeyChar;
                if (char.IsControl(ch)) continue;

                sb.Append(ch);
                Console.Write('*');
            } while (key.Key != ConsoleKey.Enter);

            return sb.ToString();
        }


        public IAuthPromptResponse Response { get; private set; } = new QuitAuthPromptResponse();
    }
}
