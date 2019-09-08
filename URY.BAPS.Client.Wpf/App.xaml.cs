using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Protocol.V2.Io;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MainWindow? _main;

        private ILifetimeScope? _diScope;

        static App()
        {
            DispatcherHelper.Initialize();
        }

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

            _main = Resolve<MainWindow>();
            _main.Show();

            SetupServerConnection();
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

        private void SetupServerConnection()
        {
            var core = Resolve<ClientCore>();
            var cache = Resolve<ConfigCache>();
            cache.SubscribeToReceiver(core.Updater);

            var auth = Resolve<Authenticator<TcpConnection>>();
            var socket = auth.Run();
            if (socket is null)
            {
                Shutdown();
                return;
            }

            core.Launch(socket, socket);
            var init = Resolve<InitialUpdatePerformer>();
            init.Run();
        }

        private static IContainer SetupDependencyContainer(IClientConfigManager configManager)
        {
            var builder = new DependencyContainerBuilder(configManager);
            return builder.Build();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var core = _diScope.ResolveOptional<ClientCore>();
            core?.Shutdown();

            _diScope?.Dispose();
        }

        private T Resolve<T>()
        {
            return _diScope.Resolve<T>();
        }
    }
}