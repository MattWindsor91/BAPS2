using System.Collections.Immutable;
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
        
        private ILogger<T> CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
        

        public ServerConfig FromConfiguration(IConfigurationRoot configuration)
        {
            var listenSection = configuration.GetSection("listen");
            var listenConfig = ListenFromConfiguration(listenSection);
            
            var channelSetSection = configuration.GetSection("channels");
            var channelSetConfig = ChannelSetFromConfiguration(channelSetSection);

            var logger = CreateLogger<ServerConfig>();
            return new ServerConfig(listenConfig, channelSetConfig, logger);
        }

        /// <summary>
        ///     Constructs a listen config by binding its corresponding section
        ///     in the BAPS server configuration.
        /// </summary>
        /// <param name="listenSection">The section of the main BAPS configuration that contains listen config.</param>
        /// <returns>The created config node.</returns>
        private ListenConfig ListenFromConfiguration(IConfigurationSection listenSection)
        {
            var logger = CreateLogger<ListenConfig>();
            var listenConfig = new ListenConfig(logger);

            listenSection.Bind(listenConfig);
            return listenConfig;
        }
        
        /// <summary>
        ///     Constructs a channel-set config by binding its corresponding section
        ///     in the BAPS server configuration.
        /// </summary>
        /// <param name="channelSetSection">The section of the main BAPS configuration that contains channel-set config.</param>
        /// <returns>The created config node.</returns>
        private ChannelSetConfig ChannelSetFromConfiguration(IConfigurationSection channelSetSection)
        {
            var logger = CreateLogger<ChannelSetConfig>();
            var channelSetConfig = new ChannelSetConfig(logger);

            channelSetSection.Bind(channelSetConfig);

            var channels = new ChannelConfig[channelSetConfig.Count];
            // todo: populate
            channelSetConfig.Channels = ImmutableArray.Create(channels);
            
            return channelSetConfig;
        }
    }
}