using System;
using Autofac;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Auth;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Client.Wpf.Auth;
using URY.BAPS.Client.Wpf.Services;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Encapsulates the whole process of registering all of the BAPS
    ///     clients' dependencies with its dependency container, such that
    ///     they can be retrieved through it later.
    /// </summary>
    public class DependencyContainerBuilder
    {
        private readonly ContainerBuilder _builder;
        private readonly IClientConfigManager _configManager;
        private IContainer? _container;

        /// <summary>
        ///     Constructs a dependency container builder.
        /// </summary>
        /// <param name="configManager">The pre-built config manager to register with the container.</param>
        public DependencyContainerBuilder(IClientConfigManager configManager)
        {
            _builder = new ContainerBuilder();
            _configManager = configManager;
        }

        /// <summary>
        ///     Builds the dependency container.
        /// </summary>
        /// <returns>
        ///     The built <see cref="IContainer"/>; if this method is called
        ///     multiple times, the same container will be returned.
        /// </returns>
        public IContainer Build()
        {
            return _container ??= ActuallyBuild(); 
        }

        private IContainer ActuallyBuild()
        {
            RegisterCoreClasses();
            RegisterControllers();
            RegisterServices();
            RegisterViewModels();
            RegisterRest();
            return _builder.Build();
        }

        private void RegisterCoreClasses()
        {
            _builder.RegisterInstance(_configManager).As<IClientConfigManager>();
            _builder.RegisterType<ClientCore>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<ConfigCache>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<InitialUpdatePerformer>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the controllers and controller sets used by the BAPS client.
        /// </summary>
        private void RegisterControllers()
        {
            _builder.RegisterType<ConfigController>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<SystemController>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<ChannelControllerSet>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<DirectoryControllerSet>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the services used by the BAPS client.
        /// </summary>
        private void RegisterServices()
        {
            _builder.RegisterType<AudioWallService>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<ChannelFactoryService>().AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<DirectoryFactoryService>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the view models used by the BAPS client.
        /// </summary>
        private void RegisterViewModels()
        {
            _builder.RegisterType<TextViewModel>().As<ITextViewModel>();
            _builder.RegisterType<MainViewModel>();
            _builder.RegisterType<LoginViewModel>();
        }

        /// <summary>
        ///     Registers parts of the BAPS client that don't easily fit into
        ///     the above categories.
        /// </summary>
        private void RegisterRest()
        {
            _builder.Register(c => new MainWindow { DataContext = c.Resolve<MainViewModel>() }).AsSelf().InstancePerLifetimeScope();
            _builder.RegisterType<DialogLoginPrompter>().As<ILoginPrompter>().InstancePerLifetimeScope();
            _builder.RegisterType<MessageBoxLoginErrorHandler>().As<ILoginErrorHandler>().InstancePerLifetimeScope();

            _builder.RegisterType<BapsAuthedConnectionBuilder>().As<IAuthedConnectionBuilder<TcpConnection>>()
                .InstancePerLifetimeScope();
            _builder.RegisterType<Authenticator<TcpConnection>>().AsSelf().InstancePerLifetimeScope();
        }
    }
}