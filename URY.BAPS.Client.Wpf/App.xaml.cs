using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Configuration;
using URY.BAPS.Client.Autofac;
using URY.BAPS.Client.Common.ClientConfig;

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
            var container = SetupDependencyContainer();
            _diScope = container.BeginLifetimeScope();

            var configManager = Resolve<IClientConfigManager>();

            var config = TryGetConfig(configManager);
            if (config is null)
            {
                Shutdown();
                return;
            }

            _main = Resolve<MainWindow>();
            _main.Show();

            if (!Resolve<Protocol.V2.Core.Client>().Start()) Shutdown();
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

        private static IContainer SetupDependencyContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ClientModule>().RegisterModule<WpfModule>();
            return builder.Build();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var client = _diScope.ResolveOptional<Protocol.V2.Core.Client>();
            client?.Stop();

            _diScope?.Dispose();
        }

        private T Resolve<T>()
        {
            return _diScope.Resolve<T>();
        }
    }
}