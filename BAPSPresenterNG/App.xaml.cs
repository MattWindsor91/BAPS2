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
        [CanBeNull, UsedImplicitly]  private ConfigMessengerAdapter _cma;
        [CanBeNull] private ClientCore _core;
        [CanBeNull] private MainWindow _main;
        [CanBeNull, UsedImplicitly]  private ReceiverMessengerAdapter _rma;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private static ConfigCache ConfigCache => SimpleIoc.Default.GetInstance<ConfigCache>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SimpleIoc.Default.Register<ConfigManager>();
            SimpleIoc.Default.Register(MakeAuthenticator);
            SimpleIoc.Default.Register<ClientCore>();

            _main = new MainWindow();
            _main.Show();

            _core = SimpleIoc.Default.GetInstance<ClientCore>();
            Debug.Assert(_core != null, nameof(_core) + " != null");
            _core.AboutToAuthenticate += AboutToAuthenticate;
            _core.AboutToAutoUpdate += ChannelCountReady;
            _core.ReceiverCreated += HandleReceiverCreated;

            var launchedProperly = _core.Launch();
            if (!launchedProperly) Shutdown();
        }

        private static void ChannelCountReady(object sender, (int numChannelsPrefetch, int numDirectoriesPrefetch) args)
        {
            // Manually pumping 'count changed' messages.
            var messenger = ViewModelLocator.Messenger;
            var (numChannelsPrefetch, numDirectoriesPrefetch) = args;
            messenger.Send(new ConfigCache.IntChangeEventArgs(OptionKey.ChannelCount, numChannelsPrefetch));
            messenger.Send(new ConfigCache.IntChangeEventArgs(OptionKey.ChannelCount, numDirectoriesPrefetch));
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

        /// <summary>
        ///     Handle the client core's <see cref="ClientCore.ReceiverCreated" />
        ///     event.
        ///     <para>
        ///         This does two things: first, it tells the config cache to
        ///         listen to server updates involving server configuration;
        ///         second, it installs some objects that convert server and
        ///         config updates to message bus messages.
        ///     </para>
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">The newly-created <see cref="Receiver" />.</param>
        private void HandleReceiverCreated(object sender, Receiver e)
        {
            ConfigCache.InstallReceiverEventHandlers(e);

            // These objects listen to events on various parts of the BAPS
            // client, and convert them to messages on the messenger bus.
            // The various view models then register onto the bus, meaning that
            // they receive server and config updates without directly wiring
            // them together.
            var messenger = ViewModelLocator.Messenger;
            _rma = new ReceiverMessengerAdapter(e, messenger);
            _cma = new ConfigMessengerAdapter(ConfigCache, messenger);
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