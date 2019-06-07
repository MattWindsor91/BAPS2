using System;
using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Auth;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Client.Wpf.Auth;
using URY.BAPS.Client.Wpf.Dialogs;
using URY.BAPS.Client.Wpf.Services;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MainWindow? _main;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private IClientConfigManager MakeClientConfigManager()
        {
            var builder = new ConfigurationBuilder();
            builder.AddIniFile("bapspresenter.ini");
            return new NetcoreConfigManager(builder);
        }

        private ClientConfig? TryGetConfig(IClientConfigManager manager)
        {
            try
            {
                return manager.LoadConfig();
            }
            catch (ClientConfigException e)
            {
                MessageBox.Show(e.Message, "Client configuration error:", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private ILifetimeScope? _diScope;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var configManager = MakeClientConfigManager();

            var config = TryGetConfig(configManager);
            if (config is null)
            {
                Shutdown();
                return;
            }

            var container = SetupDependencyContainer(configManager);
            _diScope = container.BeginLifetimeScope();

            var vm = _diScope.Resolve<MainViewModel>();
            _main = new MainWindow { DataContext = vm };
            _main.Show();

            SetupServerConnection();
        }

        private void SetupServerConnection()
        {
            var core = _diScope.Resolve<IClientCore>();
            var cache = _diScope.Resolve<ConfigCache>();
            cache.SubscribeToReceiver(core.Updater);

            var auth = _diScope.Resolve<Authenticator<TcpConnection>>();
            var socket = auth.Run();
            if (socket is null)
            {
                Shutdown();
                return;
            }

            core.Launch(socket);
            var init = _diScope.Resolve<InitialUpdatePerformer>();
            init.Run();
        }

        private IContainer SetupDependencyContainer(IClientConfigManager configManager)
        {
            var builder = new ContainerBuilder();
            RegisterCoreClasses(builder, configManager);
            RegisterControllers(builder);
            RegisterServices(builder);
            RegisterViewModels(builder);
            RegisterRest(builder);
            return builder.Build();
        }

        private void RegisterCoreClasses(ContainerBuilder builder, IClientConfigManager configManager)
        {
            builder.RegisterInstance(configManager).As<IClientConfigManager>();
            builder.RegisterType<ClientCore>().As<IClientCore>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigCache>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InitialUpdatePerformer>().AsSelf().InstancePerLifetimeScope();
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SystemController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ChannelControllerSet>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DirectoryControllerSet>().AsSelf().InstancePerLifetimeScope();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<AudioWallService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ChannelFactoryService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DirectoryFactoryService>().AsSelf().InstancePerLifetimeScope();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<TextViewModel>().As<ITextViewModel>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
        }

        private void RegisterRest(ContainerBuilder builder)
        {
            builder.Register(c => new MainWindow { DataContext = c.Resolve<MainViewModel>()}).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DialogLoginPrompter>().As<ILoginPrompter>().InstancePerLifetimeScope();
            builder.RegisterType<MessageBoxLoginErrorHandler>().As<ILoginErrorHandler>().InstancePerLifetimeScope();

            builder.RegisterType<BapsAuthedConnectionBuilder>().As<IAuthedConnectionBuilder<TcpConnection>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<Authenticator<TcpConnection>>().AsSelf().InstancePerLifetimeScope();
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _diScope?.Dispose();
        }
    }
}