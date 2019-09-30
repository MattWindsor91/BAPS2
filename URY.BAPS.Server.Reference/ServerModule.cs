using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using URY.BAPS.Server.Config;
using URY.BAPS.Server.Io;
using URY.BAPS.Server.Managers;

namespace URY.BAPS.Server
{
    /// <summary>
    ///     Autofac module that registers the various BAPS server sub-services.
    /// </summary>
    public class ServerModule : Module
    {
        public ServerConfig? ServerConfig { get; set; }
        public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(LoggerFactory).As<ILoggerFactory>().SingleInstance();

            RegisterConfig(builder);
            RegisterManagers(builder);

            builder.RegisterType<TcpServer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Server>().AsSelf().InstancePerLifetimeScope();
        }

        private void RegisterConfig(ContainerBuilder builder)
        {
            builder.RegisterInstance(ServerConfig).AsSelf().SingleInstance();
            builder.Register(r => r.Resolve<ServerConfig>().Listen).As<ListenConfig>().InstancePerLifetimeScope();
        }

        private static void RegisterManagers(ContainerBuilder builder)
        {
            builder.RegisterType<ClientManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ConfigManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<UserManager>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
