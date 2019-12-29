using System;
using System.Net.Sockets;
using Autofac;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.ServerSelect;
using URY.BAPS.Client.Protocol.V2.Auth;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Client.Protocol.V2.Decode;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.MessageIo;

namespace URY.BAPS.Client.Autofac
{
    /// <summary>
    ///     A basic Autofac module for BAPS clients.
    ///     <para>
    ///         This module can be configured then used to bring up most of
    ///         the components of a BAPS client.
    ///     </para>
    /// </summary>
    public class ClientModule : Module
    {
        /// <summary>
        ///     The client configuration.
        /// </summary>
        public ClientConfig Config { get; set; } = new ClientConfig();

        // TODO(@MattWindsor91): BAPS protocol selection (V2 or V3)

        protected override void Load(ContainerBuilder builder)
        {
            RegisterConfig(builder);
            RegisterCoreComponents(builder);
            RegisterServerSelectComponents(builder);
            RegisterAuthComponents(builder);
            RegisterControllerComponents(builder);
        }

        public static ClientModule WithConfigFromIniFile(string path = "bapspresenter.ini")
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Environment.CurrentDirectory).AddIniFile(path);
            var configManager = new NetcoreConfigManager(builder);
            return new ClientModule {Config = configManager.LoadConfig() };
        }

        /// <summary>
        ///     Registers the BAPS client config for dependency injection.
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private void RegisterConfig(ContainerBuilder builder)
        {
            builder.RegisterInstance(Config).AsSelf().SingleInstance();
            builder.RegisterInstance(Config.Servers).AsSelf().SingleInstance();
        }

        /// <summary>
        ///     Registers the core components used by the BAPS client.
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private static void RegisterCoreComponents(ContainerBuilder builder)
        {
            RegisterProtocolV2ConnectionComponents(builder);

            builder.Register(c => c.Resolve<MessageConnectionManager>().EventFeed).As<IFullEventFeed>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ConfigCache>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InitialUpdatePerformer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<V2Client>().AsSelf().InstancePerLifetimeScope();
        }

        private static void RegisterProtocolV2ConnectionComponents(ContainerBuilder builder)
        {
            builder.RegisterType<MessageConnectionManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ClientCommandDecoder>().As<CommandDecoder>().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the core server selection components used by the BAPS client.
        ///     <para>
        ///         This doesn't register an <see cref="IServerPrompter"/> or a <see cref="IServerErrorHandler"/>, as
        ///         they will be client-specific.
        ///     </para>
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private static void RegisterServerSelectComponents(ContainerBuilder builder)
        {
            builder.RegisterType<ServerSelector>().AsSelf().InstancePerLifetimeScope();
        }
        
        /// <summary>
        ///     Registers the core authenticator components used by the BAPS client.
        ///     <para>
        ///         This doesn't register an <see cref="IAuthPrompter"/> or a <see cref="ILoginErrorHandler"/>, as they
        ///         will be client-specific.
        ///     </para>
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private static void RegisterAuthComponents(ContainerBuilder builder)
        {
             builder.RegisterType<V2HandshakePerformer>().As<IHandshakePerformer<SeededPrimitiveConnection>>()
                 .InstancePerLifetimeScope();
             builder.RegisterType<V2AuthPerformer>().As<IAuthPerformer<SeededPrimitiveConnection,IMessageConnection>>()
                 .InstancePerLifetimeScope();           
             builder.RegisterType<LoginPerformer<SeededPrimitiveConnection,IMessageConnection>>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the controllers and controller sets used by the BAPS client.
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private static void RegisterControllerComponents(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SystemController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ChannelControllerSet>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DirectoryControllerSet>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
