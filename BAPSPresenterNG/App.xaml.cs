using System;
using System.Diagnostics;
using System.Windows;
using BAPSClientCommon;
using BAPSClientCommon.ServerConfig;
using BAPSClientWindows;
using BAPSPresenterNG.ViewModel;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        [CanBeNull] private IClientCore _core;
        [CanBeNull] private MainWindow _main;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private static ConfigCache ConfigCache => SimpleIoc.Default.GetInstance<ConfigCache>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SimpleIoc.Default.Register<ConfigManager>();
            SimpleIoc.Default.Register(MakeAuthenticator);
            SimpleIoc.Default.Register<IClientCore, ClientCore>();

            _main = new MainWindow();
            _main.Show();
            
            _core = ViewModelLocator.ClientCore;
            Debug.Assert(_core != null, nameof(_core) + " != null");
            ConfigCache.InstallReceiverEventHandlers(_core);
            _core.AboutToAuthenticate += AboutToAuthenticate;
            _core.AboutToAutoUpdate += ChannelCountReady;
            
            var launchedProperly = _core.Launch();
            if (!launchedProperly) Shutdown();
        }

        private static void ChannelCountReady(object sender, (int numChannelsPrefetch, int numDirectoriesPrefetch) args)
        {
            // Manually pumping 'count changed' messages.
            var (numChannelsPrefetch, numDirectoriesPrefetch) = args;
            ConfigCache.AddOptionDescription((uint)OptionKey.ChannelCount, ConfigType.Int, "Number of channels", false);
            ConfigCache.AddOptionValue((uint)OptionKey.ChannelCount, numChannelsPrefetch);
            ConfigCache.AddOptionDescription((uint)OptionKey.DirectoryCount, ConfigType.Int, "Number of directories", false);
            ConfigCache.AddOptionValue((uint)OptionKey.DirectoryCount, numDirectoriesPrefetch);
        }

        private Authenticator MakeAuthenticator()
        {
            return new Authenticator(LoginCallback);
        }

        private Authenticator.Response LoginCallback()
        {
            var login = new Login
            {
                Owner = _main
            };
            if (login.ShowDialog() == false)
                return new Authenticator.Response {IsGivingUp = true};

            return new Authenticator.Response
            {
                IsGivingUp = false,
                Username = login.ViewModel.Username,
                Password = login.passwordTxt.Password,
                Server = login.ViewModel.Server,
                Port = login.ViewModel.Port
            };
        }

        private void AboutToAuthenticate(object sender, [NotNull] Authenticator e)
        {
            e.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButton.OK);
                //logError(errorMessage);
            };
            e.UserError += (s, errorMessage) => { MessageBox.Show(errorMessage, "Login error:", MessageBoxButton.OK); };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _core?.Dispose();
        }
    }
}