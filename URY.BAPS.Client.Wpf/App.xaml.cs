using System;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using URY.BAPS.Client.Common.ClientConfig;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Client.Windows;
using URY.BAPS.Client.Wpf.Dialogs;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IClientCore? _core;
        private MainWindow? _main;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private static ConfigCache ConfigCache => SimpleIoc.Default.GetInstance<ConfigCache>();

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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var configManager = MakeClientConfigManager();

            if (TryGetConfig(configManager) is {} config)
            {
                RegisterDependencies(configManager);
            }
            else
            {
                Shutdown();
                return;
            }

            _main = new MainWindow();
            _main.Show();

            _core = ViewModelLocator.ClientCore;
            ConfigCache.SubscribeToReceiver(_core.Updater);

            var launchedProperly = _core.Launch();
            if (!launchedProperly)
            {
                Shutdown();
                return;
            }

            var init = SimpleIoc.Default.GetInstance<InitialUpdatePerformer>();
            init.Run();
        }

        private void RegisterDependencies(IClientConfigManager configManager)
        {
            SimpleIoc.Default.Register(() => configManager);
            SimpleIoc.Default.Register(MakeAuthenticator);
            SimpleIoc.Default.Register<IClientCore, ClientCore>();
            SimpleIoc.Default.Register<InitialUpdatePerformer>();
        }

        private Authenticator MakeAuthenticator()
        {
            var auth = new Authenticator(LoginCallback);
            auth.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            auth.UserError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            return auth;
        }

        private Authenticator.Response LoginCallback()
        {
            var login = new Login
            {
                Owner = _main
            };
            if (login.ShowDialog() == false)
                return new Authenticator.Response {IsGivingUp = true};

            var vm = login.ViewModel ?? throw new NullReferenceException("View model was null.");

            return new Authenticator.Response
            {
                IsGivingUp = false,
                Username = vm.Username,
                Password = login.PasswordTxt.Password,
                Server = vm.Server,
                Port = vm.Port
            };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _core?.Dispose();
        }
    }
}