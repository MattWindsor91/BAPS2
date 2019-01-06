using BAPSCommon;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigManager.initConfigManager();

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
            SetupPlaybackReactions(e);
        }

        private void SetupPlaybackReactions(Receiver e)
        {
            foreach (var channel in _main.ViewModel.Channels)
            {
                channel.SetupPlaybackReactions(e);
            }
        }

        private void Authenticated(object sender, EventArgs e)
        {
            for (ushort i = 0; i < 3; i++)
            {
                var channel = new ChannelViewModel(i)
                {
                    Controller = new ChannelController(i, _core.SendQueue)
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
