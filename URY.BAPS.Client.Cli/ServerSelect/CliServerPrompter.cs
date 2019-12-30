using System;
using System.Globalization;
using System.Linq;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerSelect;
using static URY.BAPS.Client.Cli.CliUtilities;

namespace URY.BAPS.Client.Cli.ServerSelect
{
    /// <summary>
    ///     A <see cref="IServerPrompter" /> that prompts the user for a server using the command line.
    /// </summary>
    public class CliServerPrompter : IServerPrompter
    {
        private readonly ServerPoolConfig _serverPool;

        public CliServerPrompter(ServerPoolConfig serverPool)
        {
            _serverPool = serverPool;
        }

        private ServerRecord[] ServerRecords => _serverPool.Records;

        private ServerRecord DefaultRecord => _serverPool.DefaultRecord;

        public ServerRecord Selection { get; private set; } = ServerRecord.Empty();

        public bool GaveUp { get; private set; }

        public void Prompt()
        {
            ListServers();

            Console.Out.WriteLine(
                "Type the number of a server above, 'custom' to specify a custom server, or 'quit' to exit.");

            var defaultValid = DefaultRecord.IsValid;
            if (defaultValid) Console.Out.WriteLine("Leave the line blank to select the default (*) server.");

            var chosen = false;
            while (!chosen)
            {
                var rawLine = ReadLine.Read("Choice: ");
                var line = rawLine.ToLowerInvariant().Trim();

                chosen = true;
                switch (line)
                {
                    case "custom":
                        PromptForCustomServer();
                        break;
                    case "default" when DefaultRecord.IsValid:
                        Selection = DefaultRecord;
                        break;
                    case "quit":
                        GaveUp = true;
                        break;
                    case var _ when TryParseServerIndex(line, out var pos):
                        Selection = ServerRecords[pos - 1];
                        break;
                    default:
                        chosen = false;
                        Console.Error.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private bool TryParseServerIndex(string line, out int pos)
        {
            return int.TryParse(line, NumberStyles.None, CultureInfo.InvariantCulture, out pos) && 0 < pos &&
                   pos <= ServerRecords.Length;
        }

        private void ListServers()
        {
            Console.Out.WriteLine($"Listing {ServerCountDescription(ServerRecords.Length)}.");
            foreach (var (server, i) in ServerRecords.Zip(Enumerable.Range(1, ServerRecords.Length)))
                ListServer(server, i);
        }

        private void ListServer(ServerRecord server, int position)
        {
            Console.Out.Write(_serverPool.Default == server.Name ? "* " : "   ");
            Console.Out.Write(position);
            Console.Out.Write(") ");
            WriteName(server.Name, server.Colour);
            Console.Out.Write($" ({server.Host}:{server.Port})");
            WriteValidity(server);
            Console.Out.WriteLine();
        }

        private static void WriteValidity(ServerRecord server)
        {
            if (server.IsValid) return;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Out.Write(" <INVALID>");
            Console.ResetColor();
        }

        private static void WriteName(string name, ServerColour colour)
        {
            Console.ForegroundColor = colour.ToConsoleColor();
            Console.Out.Write(name);
            Console.ResetColor();
        }


        private static string ServerCountDescription(int length)
        {
            return length switch
            {
                0 => "no servers",
                1 => "1 server",
                _ => $"{length} servers"
            };
        }

        private void PromptForCustomServer()
        {
            var serverAddress = GetServerAddress();
            var serverPort = GetServerPort();

            Selection = new ServerRecord
                {Name = "Custom Server", Host = serverAddress, Port = serverPort, Colour = ServerColour.None};
        }

        private string GetServerAddress()
        {
            return GetText("Server address", DefaultRecord.Host);
        }

        private int GetServerPort()
        {
            var culture = CultureInfo.InvariantCulture;

            var portString = GetText("Server port", DefaultRecord.Port.ToString(culture));
            return int.Parse(portString, NumberStyles.None, culture);
        }
    }
}