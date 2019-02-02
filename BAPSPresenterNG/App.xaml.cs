using BAPSClientCommon;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using BAPSPresenterNG.Controls;
using BAPSPresenterNG.ViewModel;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ClientCore _core;
        private MainWindow _main;
        private ReceiverMessengerAdapter _rma;
        private ChannelControllerMessengerAdapter _cma;

        static App()
        {
            DispatcherHelper.Initialize();
        }
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SimpleIoc.Default.Register<BAPSClientWindows.ConfigManager>();
            SimpleIoc.Default.Register<ConfigCache>();
            SimpleIoc.Default.Register(MakeAuthenticator);
            SimpleIoc.Default.Register<ClientCore>();
            
            _main = new MainWindow();
            _main.Show();

            _core = SimpleIoc.Default.GetInstance<ClientCore>();
            _core.AboutToAuthenticate += AboutToAuthenticate;
            _core.Authenticated += Authenticated;
            _core.ReceiverCreated += ReceiverCreated;

            var launchedProperly = _core.Launch();
            if (!launchedProperly) Shutdown();
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
                return new Authenticator.Response { IsGivingUp = true };

            return new Authenticator.Response
            {
                IsGivingUp = false,
                Username = login.ViewModel.Username,
                Password = login.passwordTxt.Password,
                Server = login.ViewModel.Server,
                Port = login.ViewModel.Port
            };
        }

        private void ReceiverCreated(object sender, Receiver e)
        {
            var messenger = Messenger.Default;

            _rma = new ReceiverMessengerAdapter(e, messenger);
            _rma.Register();

            // TODO(@MattWindsor91): replace this with messages
            var mainViewModel = SimpleIoc.Default.GetInstance<ViewModel.MainViewModel>();
            mainViewModel.Register(messenger);            
            SetupConfigReactions(e);
        }

        private void SetupConfigReactions(IConfigServerUpdater r)
        {
            ConfigCache.InstallReceiverEventHandlers(r);
        }

        private ConfigCache ConfigCache => SimpleIoc.Default.GetInstance<ConfigCache>();

        private void Authenticated(object sender, EventArgs e)
        {
            var locator = (ViewModel.ViewModelLocator) Resources["Locator"];
            locator.RegisterChannels(ClientCore.NumChannels);
            
            var mainViewModel = SimpleIoc.Default.GetInstance<ViewModel.MainViewModel>();
            var messenger = Messenger.Default;

            _cma = new ChannelControllerMessengerAdapter(_core.ControllerFor, messenger);
            _cma.Register();

            foreach (var c in locator.Channels) mainViewModel.Channels.Add(c);
        }

        private void AboutToAuthenticate(object sender, Authenticator e)
        {
            e.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButton.OK);
                //logError(errorMessage);
            };
            e.UserError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Login error:", MessageBoxButton.OK);
            };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _core?.Dispose();
        }
    }
}
