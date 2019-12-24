using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using URY.BAPS.Server.Config;
using URY.BAPS.Server.Io;
using URY.BAPS.Server.Managers;
using URY.BAPS.Server.Model;

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
            RegisterModels(builder);
            RegisterManagers(builder);

            builder.RegisterType<TcpServer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Server>().AsSelf().InstancePerLifetimeScope();
        }

        private void RegisterConfig(ContainerBuilder builder)
        {
            builder.RegisterInstance(ServerConfig).AsSelf().SingleInstance();
            builder.Register(r => r.Resolve<ServerConfig>().Listen).As<ListenConfig>().InstancePerLifetimeScope();
            builder.Register(r => r.Resolve<ServerConfig>().ChannelSet).As<ChannelSetConfig>().InstancePerLifetimeScope();
        }

        private void RegisterModels(ContainerBuilder builder)
        {
            // Each channel takes a playlist, but, while each playlist must be
            // distinct, it doesn't depend on the channel configuration.
            builder.RegisterType<Playlist>().AsSelf().InstancePerDependency();

            // So far, the player also doesn't depend on the channel
            // configuration, but this will likely change.
            builder.RegisterType<Player>().AsSelf().InstancePerDependency();
            
            RegisterChannels(builder);

            builder.RegisterType<ChannelSet>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers a service for each channel in the module's channel
        ///     configuration.
        /// </summary>
        /// <param name="builder">
        ///     The builder for the container into which the channels are being
        ///     registered.
        /// </param>
        private void RegisterChannels(ContainerBuilder builder)
        {
            var channels = ServerConfig?.ChannelSet?.Channels ?? ImmutableArray<ChannelConfig>.Empty;
            foreach (var channel in channels)
            {
                builder.RegisterType<Channel>().AsSelf().WithParameter("config", channel);
            }
        }

        private static void RegisterManagers(ContainerBuilder builder)
        {
            builder.RegisterType<ClientManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ConfigManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<UserManager>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
