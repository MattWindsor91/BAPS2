using System;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    /// <summary>
    ///     Base interface of all BAPS config tree nodes.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        ///     Dumps this config node to its attached logger.
        /// </summary>
        void DumpToLogger();
    }

    /// <summary>
    ///     Root of the BAPS server config tree.
    /// </summary>
    public class ServerConfig : IConfig
    {
        private ILogger<ServerConfig> _logger;
        
        public ListenConfig Listen { get; }

        public ServerConfig(ListenConfig listen, ILogger<ServerConfig> logger)
        {
            Listen = listen;
            _logger = logger;
        }

        public void DumpToLogger()
        {
            _logger.LogInformation("Here comes the BAPS server configuration.");
            Listen.DumpToLogger();
        }
    }
}