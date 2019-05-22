using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using URY.BAPS.Server.Reference.Config;

namespace URY.BAPS.Server.Reference
{
    internal class Program
    {
        private readonly string[] _args;
        private IContainer _container;

        private static IContainer BuildContainer()
        {
            var cb = new ContainerBuilder();
            return cb.Build();
        }
        
        private Program(string[] args)
        {
            _args = args;
            _container = BuildContainer();
        }

        private void Run()
        {
            using var scope = _container.BeginLifetimeScope();
            
            var configuration = MakeConfiguration();
            var loggerFactory = MakeLoggerFactory(configuration);
            var configFactory = new ConfigFactory(loggerFactory);
            var config = configFactory.FromConfiguration(configuration);

            config.DumpToLogger();
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