using BAPSCommon;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ClientCore _core;
        private MainWindow _main;
        private ReceiverMessengerAdapter _rma;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigManager.initConfigManager();
            SimpleIoc.Default.Register<ConfigCache>();

            _main = new MainWindow();
            _main.Show();

            _core = new ClientCore(LoginCallback);
            _core.AboutToAuthenticate += AboutToAuthenticate;
            _core.Authenticated += Authenticated;
            _core.ReceiverCreated += ReceiverCreated;

            var launchedProperly = _core.Launch();
            if (!launchedProperly)
            {
                Shutdown();
                return;
            }
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
            _rma = new ReceiverMessengerAdapter(e, Messenger.Default);
            _rma.Register();

            foreach (var channel in _main.ViewModel.Channels)
            {
                channel.SetupReactions(e);
            }

            SetupConfigReactions(e);
        }

        private void SetupConfigReactions(Receiver r)
        {
            ConfigCache.InstallReceiverEventHandlers(r);
        }

        private ConfigCache ConfigCache => SimpleIoc.Default.GetInstance<ConfigCache>();

        private void Authenticated(object sender, EventArgs e)
        {
            var config = SimpleIoc.Default.GetInstance<ConfigCache>();

            for (ushort i = 0; i < 3; i++)
            {
                var channel = new ViewModel.ChannelViewModel(i)
                {
                    Controller = new ChannelController(i, _core.SendQueue, config)
                };
                _main.ViewModel.Channels.Add(channel);
            }
        }

        private void AboutToAuthenticate(object sender, Authenticator e)
        {
            e.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButton.OK);
                //logError(errorMessage);
            };
            e.UserError += (s, ErrorMessage) =>
            {
                MessageBox.Show(ErrorMessage, "Login error:", MessageBoxButton.OK);
            };
        }

        private CancellationTokenSource dead = new CancellationTokenSource();

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _core?.Dispose();
        }
    }
}
