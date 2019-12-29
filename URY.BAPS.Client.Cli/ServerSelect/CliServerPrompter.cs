using System.Globalization;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerSelect;
using static URY.BAPS.Client.Cli.CliUtilities;

namespace URY.BAPS.Client.Cli.ServerSelect
{
    /// <summary>
    ///     A <see cref="IServerPrompter"/> that prompts the user for a server using the command line.
    /// </summary>
    public class CliServerPrompter : IServerPrompter
    {
        private ServerRecord _selection = ServerRecord.Empty();
        private readonly ServerPoolConfig _serverPool;

        public ServerRecord Selection => _selection;

        public bool GaveUp => false;
        
        public CliServerPrompter(ServerPoolConfig serverPool)
        {
            _serverPool = serverPool;
        }

        public void Prompt()
        {
            // TODO(@MattWindsor91): show server records
            var serverAddress = GetServerAddress();
            var serverPort = GetServerPort();

            _selection = new ServerRecord
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

        private ServerRecord DefaultRecord => _serverPool.DefaultRecord;
    }
}