using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using URY.BAPS.Server.Config;

namespace URY.BAPS.Server
{
    /// <summary>
    ///     Main entry point.
    ///     <para>
    ///         The purpose of <see cref="Program"/> is to instantiate the server config, then
    ///         build and run a <see cref="Server"/> using it.
    ///     </para>
    /// </summary>
    internal class Program
    {
        private readonly string[] _args;
       
        private Program(string[] args)
        {
            _args = args;
        }

        private void Run()
        {
            var config = GetServerConfig();
            var server = new Server(config);
            server.Run();
        }

        private ServerConfig GetServerConfig()
        {
            var configuration = MakeConfiguration();
            var loggerFactory = MakeLoggerFactory(configuration);
            var configFactory = new ConfigFactory(loggerFactory);
            var config = configFactory.FromConfiguration(configuration);
            return config;
        }

        private IConfigurationRoot MakeConfiguration()
        {
            IConfigurationBuilder confBuilder = new ConfigurationBuilder();
            confBuilder = SetupConfiguration(confBuilder);
            return confBuilder.Build();
        }

        private ILoggerFactory MakeLoggerFactory(IConfiguration configuration)
        {
            var loggingSection = configuration.GetSection("logging");
            return LoggerFactory.Create(builder =>
                builder.AddConfiguration(loggingSection).AddConsole().AddDebug()
            );
        }

        private IConfigurationBuilder SetupConfiguration(IConfigurationBuilder cb)
        {
            return
                cb.SetBasePath(Directory.GetCurrentDirectory())
                    .AddIniFile("baps.ini")
                    .AddCommandLine(_args);
        }

        private static void Main(string[] args)
        {
            new Program(args).Run();
        }
    }
}