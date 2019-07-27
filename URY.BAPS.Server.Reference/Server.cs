using Autofac;
using URY.BAPS.Server.Reference.Config;

namespace URY.BAPS.Server.Reference
{
    /// <summary>
    ///     Main encapsulating class of the BAPS server.
    /// </summary>
    public class Server
    {
        /// <summary>
        ///     This server's configuration.
        /// </summary>
        private readonly ServerConfig _config;

        /// <summary>
        ///     This server's inversion-of-control container, used to acquire other server components.
        /// </summary>
        private readonly IContainer _container;

        public Server(ServerConfig config)
        {
            _config = config;
            _container = MakeContainer(config);
        }

        public void Run()
        {
            using var scope = _container.BeginLifetimeScope();
            _config.DumpToLogger();
        }

        private static IContainer MakeContainer(ServerConfig config)
        {
            var cb = new ContainerBuilder();
            return cb.Build();
        }
    }
}