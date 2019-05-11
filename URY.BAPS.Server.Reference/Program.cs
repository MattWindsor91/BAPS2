using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using URY.BAPS.Server.V3.Config;

namespace URY.BAPS.Server.V3
{
    class Program
    {
        private readonly string[] _args;

        public Program(string[] args)
        {
            _args = args;
        }
        
        public void Run()
        {
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
        
        static void Main(string[] args)
        {
            new Program(args).Run();
        }
    }
}