using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    [Serializable]
    public class InvalidConfigException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidConfigException()
        {
        }

        public InvalidConfigException(string message) : base(message)
        {
        }

        public InvalidConfigException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidConfigException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

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
            var listenConfig = MakeListen(listenSection);

            var channelSetSection = configuration.GetSection("channels");
            var channelSetConfig = MakeChannelSet(channelSetSection);

            var logger = CreateLogger<ServerConfig>();
            return new ServerConfig(listenConfig, channelSetConfig, logger);
        }

        /// <summary>
        ///     Constructs a listen config by binding its corresponding section
        ///     in the BAPS server configuration.
        /// </summary>
        /// <param name="listenSection">
        ///     The section of the main BAPS configuration that contains the listen config.
        /// </param>
        /// <returns>The created config node.</returns>
        private ListenConfig MakeListen(IConfigurationSection listenSection)
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
        private ChannelSetConfig MakeChannelSet(IConfigurationSection channelSetSection)
        {
            var logger = CreateLogger<ChannelSetConfig>();
            var channelSetConfig = new ChannelSetConfig(logger);

            channelSetSection.Bind(channelSetConfig);

            channelSetConfig.Channels = MakeChannelConfigs(channelSetSection, channelSetConfig.Count);

            return channelSetConfig;
        }

        private ImmutableArray<ChannelConfig> MakeChannelConfigs(IConfigurationSection channelSetSection,
            int numChannels)
        {
            var ids = EnumerateChannelIds(numChannels);
            var channels =
                from id in ids
                select MakeChannel(SectionOfChannelId(channelSetSection, id), id);

            return channels.ToImmutableArray();
        }

        /// <summary>
        ///     Constructs a single channel's config by binding its corresponding section
        ///     in the BAPS server configuration.
        /// </summary>
        /// <param name="id">The ID of the channel.</param>
        /// <param name="channelSection">
        ///     The section of the main BAPS configuration that contains the config
        ///     for this particular channel.
        /// </param>
        /// <returns>The created config node.</returns>
        private ChannelConfig MakeChannel(IConfiguration channelSection, byte id)
        {
            var logger = CreateLogger<ChannelConfig>();
            var channelConfig = new ChannelConfig(id, logger);

            channelSection.Bind(channelConfig);
            return channelConfig;
        }

        /// <summary>
        ///     Produces an enumeration of zero-based channel IDs.
        /// </summary>
        /// <param name="numChannels">
        ///     The number of channels that we are creating.
        /// </param>
        /// <returns>
        ///     An enumerable of channel ids from 0 inclusive to
        ///     <paramref name="numChannels" /> exclusive.
        /// </returns>
        /// <exception cref="InvalidConfigException">
        ///     Raised if the number of channels requested exceeds the number that
        ///     the BAPS protocols can safely handle.
        /// </exception>
        private static IEnumerable<byte> EnumerateChannelIds(int numChannels)
        {
            if (64 <= numChannels) throw new InvalidConfigException("Channel count too high.");

            return from id in Enumerable.Range(0, numChannels) select (byte) id;
        }

        private static IConfigurationSection SectionOfChannelId(IConfigurationSection channelSetSection, byte id)
        {
            return channelSetSection.GetSection(SectionKeyOfChannelId(id));
        }

        /// <summary>
        ///     Gets the sub-key of the channels configuration key for the given
        ///     channel ID.
        /// </summary>
        /// <param name="id">The channel ID.</param>
        /// <returns>
        ///     The section key for <paramref name="id" />, which is the ID
        ///     plus one as a decimal string.
        /// </returns>
        private static string SectionKeyOfChannelId(byte id)
        {
            return (id + 1).ToString();
        }
    }
}