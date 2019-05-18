using System.Threading;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    public class ListenConfig
    {
        private readonly ILogger<ListenConfig> _logger;

        /// <summary>
        ///     The TCP/IP host on which this server is listening.
        /// </summary>
        public string Host { get; set; } = "localhost";
        
        /// <summary>
        ///     The TCP/IP port on which this server is listening.
        /// </summary>
        public ushort Port { get; set; } = 1350;

        public Protocol Protocol { get; set; } = Protocol.BapsNetV2;

        public ListenConfig(ILogger<ListenConfig> logger)
        {
            _logger = logger;
        }

        public void DumpToLogger()
        {
            _logger.LogInformation("Listening on host {Host}, port {Port}, on BAPS protocol {Protocol}.", Host, Port, Protocol);
        }
    }
}