using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using URY.BAPS.Server.Config;

namespace URY.BAPS.Server
{
    /// <summary>
    ///     Allows running a BAPS server as a standalone program.
    /// </summary>
    public class ServerRunner : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        ///     This server's configuration.
        /// </summary>
        private readonly ServerConfig _config;

        /// <summary>
        ///     This server's dependency injection scope, used to acquire other server components.
        /// </summary>
        private readonly ILifetimeScope _diScope;



        public ServerRunner(ILoggerFactory loggerFactory, ServerConfig config)
        {
            _loggerFactory = loggerFactory;
            _config = config;
            _diScope = MakeContainer(config);
        }

        public Task Run()
        {
            _config.DumpToLogger();
            return _diScope.Resolve<Server>().Run();
        }

        private ILifetimeScope MakeContainer(ServerConfig config)
        {
            var cb = new ContainerBuilder();

            var serverModule = new ServerModule {ServerConfig = config, LoggerFactory = _loggerFactory};
            cb.RegisterModule(serverModule);
            return cb.Build().BeginLifetimeScope();
        }

        public void Dispose()
        {
            _diScope?.Dispose();
        }
    }
}