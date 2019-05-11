using System.Threading;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.V3.Config
{
    public class ListenConfig
    {
        private readonly ILogger<ListenConfig> _logger;

        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 1350;

        public ListenConfig(ILogger<ListenConfig> logger)
        {
            _logger = logger;
        }

        public void DumpToLogger()
        {
            _logger.LogInformation("Listening on host {Host}, port {Port}.", Host, Port);
        }
    }
}