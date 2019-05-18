using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    /// <summary>
    ///     Factory for building an initial BAPS server config.
    /// </summary>
    public class ConfigFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public ConfigFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ServerConfig FromConfiguration(IConfigurationRoot configuration)
        {
            var listenSection = configuration.GetSection("listen");

            var listenConfig = ListenFromConfiguration(listenSection);

            var logger = _loggerFactory.CreateLogger<ServerConfig>();

            return new ServerConfig(listenConfig, logger);
        }

        /// <summary>
        ///     Constructs a listen config by binding its corresponding section
        ///     in the BAPS server configuration.
        /// </summary>
        /// <param name="listenSection">The section of the main BAPS configuration that contains listen config.</param>
        /// <returns>The created config node.</returns>
        private ListenConfig ListenFromConfiguration(IConfigurationSection listenSection)
        {
            var logger = _loggerFactory.CreateLogger<ListenConfig>();
            var listenConfig = new ListenConfig(logger);

            listenSection.Bind(listenConfig);
            return listenConfig;
        }
    }
}