using System.IO;
using Microsoft.Extensions.Configuration;

namespace URY.BAPS.Client.Common.ClientConfig
{
    /// <summary>
    ///     A client config manager that reads its configuration from a
    ///     .NET Core <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public class NetcoreConfigManager : IClientConfigManager
    {
        private readonly IConfigurationBuilder _builder;

        /// <summary>
        ///     Creates a <see cref="NetcoreConfigManager"/>.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="IConfigurationBuilder"/> that we use to make
        ///     configuration.
        /// </param>
        public NetcoreConfigManager(IConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public ClientConfig LoadConfig()
        {
            var configuration = BuildConfiguration();
            var config = new ClientConfig();
            configuration.Bind(config);
            return config;
        }

        private IConfiguration BuildConfiguration()
        {
            try
            {
                return _builder.Build();
            }
            catch (FileNotFoundException e)
            {
                throw new ClientConfigException($"Configuration file not found: {e.Message}", e);
            }
        }
    }
}
