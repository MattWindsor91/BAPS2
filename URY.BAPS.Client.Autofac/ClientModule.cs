using System;
using System.Net.Sockets;
using Autofac;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Auth;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Client.Protocol.V2.Decode;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Commands;
using URY.BAPS.Common.Protocol.V2.Decode;
using URY.BAPS.Common.Protocol.V2.MessageIo;
using URY.BAPS.Common.Protocol.V2.PrimitiveIo;

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
        ///     The path from which the client will load configuration.
        /// </summary>
        public string ConfigPath { get; set; } = "bapspresenter.ini";

        // TODO(@MattWindsor91): BAPS protocol selection (V2 or V3)

        protected override void Load(ContainerBuilder builder)
        {
            RegisterConfigManagerComponent(builder);
            RegisterCoreComponents(builder);
            RegisterAuthComponents(builder);
            RegisterControllerComponents(builder);
        }

        private void RegisterConfigManagerComponent(ContainerBuilder builder)
        {
            builder.Register(MakeConfigManager).As<IClientConfigManager>().InstancePerLifetimeScope();
        }

        private IClientConfigManager MakeConfigManager(IComponentContext _)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Environment.CurrentDirectory).AddIniFile(ConfigPath);
            return new NetcoreConfigManager(builder);
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

            builder.Register(c => c.Resolve<DetachableConnection>().EventFeed).As<IFullEventFeed>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ConfigCache>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InitialUpdatePerformer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Protocol.V2.Core.Client>().AsSelf().InstancePerLifetimeScope();
        }

        private static void RegisterProtocolV2ConnectionComponents(ContainerBuilder builder)
        {
            builder.RegisterType<TcpConnection>().As<BAPS.Common.Protocol.V2.MessageIo.IConnection>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DetachableConnection>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ClientCommandDecoder>().As<CommandDecoder>().InstancePerLifetimeScope();
            builder.RegisterType<ConnectionFactory<DetachableConnection>>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the core authenticator components used by the BAPS client.
        ///     <para>
        ///         This doesn't register an <see cref="ILoginPrompter"/> or a
        ///         <see cref="ILoginErrorHandler"/>, as they will be
        ///         client-specific.
        ///     </para>
        /// </summary>
        /// <param name="builder">
        ///     The builder constructing the Autofac container.
        /// </param>
        private static void RegisterAuthComponents(ContainerBuilder builder)
        {
            builder.RegisterType<BapsLoginAttempter>().As<ILoginAttempter<TcpConnection>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<Authenticator<TcpConnection>>().AsSelf().InstancePerLifetimeScope();
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
