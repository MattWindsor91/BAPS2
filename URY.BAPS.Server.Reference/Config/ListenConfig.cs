using System.Net;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Config
{
    public class ListenConfig
    {
        private readonly ILogger<ListenConfig> _logger;

        public ListenConfig(ILogger<ListenConfig> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     The TCP/IP host on which this server is listening, as a raw
        ///     string.
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        ///     The TCP/IP port on which this server is listening.
        /// </summary>
        public ushort Port { get; set; } = 1350;

        /// <summary>
        ///     Retrieves the result of parsing <see cref="Host"/> as a
        ///     dotted-quad IP address.
        /// </summary>
        public IPAddress HostAddress => IPAddress.Parse(Host);

        /// <summary>
        ///     Retrieves the endpoint defined by <see cref="HostAddress"/>
        ///     and <see cref="Port"/>.
        /// </summary>
        public IPEndPoint HostEndPoint => new IPEndPoint(HostAddress, Port);

        public Protocol Protocol { get; set; } = Protocol.BapsNetV2;

        public void DumpToLogger()
        {
            _logger.LogInformation("Listening on host {Host}, port {Port}, on BAPS protocol {Protocol}.", Host, Port,
                Protocol);
        }
    }
}