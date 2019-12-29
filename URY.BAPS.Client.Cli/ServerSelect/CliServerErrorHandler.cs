using System;
using System.Net.Sockets;
using URY.BAPS.Client.Common.ServerSelect;

namespace URY.BAPS.Client.Cli.ServerSelect
{
    /// <summary>
    ///     A <see cref="IServerErrorHandler"/> that outputs errors to standard error.
    /// </summary>
    public class CliServerErrorHandler : IServerErrorHandler
    {
        public void HandleGivingUp()
        {
            Console.Error.WriteLine("Giving up on selecting a server.");
        }

        public void HandleInvalidServer(ServerRecord invalid)
        {
            Console.Error.WriteLine("Invalid server.");
        }

        public void HandleSocketException(SocketException exc)
        {
            Console.Error.WriteLine($"Couldn't connect to server: {exc.Message}");
        }
    }
}