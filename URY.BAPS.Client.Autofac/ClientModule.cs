using Autofac;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Auth;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Model.EventFeed;
using URY.BAPS.Common.Protocol.V2.Io;

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
            builder.AddIniFile(ConfigPath);
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
            builder.RegisterType<ConnectionManager>().AsSelf().InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<ConnectionManager>().EventFeed).As<IFullEventFeed>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ConfigCache>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InitialUpdatePerformer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<Protocol.V2.Core.Client>().AsSelf().InstancePerLifetimeScope();
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
            builder.RegisterType<BapsAuthedConnectionBuilder>().As<IAuthedConnectionBuilder<TcpConnection>>()
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
