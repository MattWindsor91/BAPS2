using URY.BAPS.Client.Common.ServerConfig;
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
            SimpleIoc.Default.Register<InitialUpdatePerformer>();

            _main = new MainWindow();
            _main.Show();

            _core = ViewModelLocator.ClientCore;
            Debug.Assert(_core != null, nameof(_core) + " != null");
            ConfigCache.SubscribeToReceiver(_core);

            var launchedProperly = _core.Launch();
            if (!launchedProperly) Shutdown();

            var init = SimpleIoc.Default.GetInstance<InitialUpdatePerformer>();
            init.Run();
        }

        private Authenticator MakeAuthenticator()
        {
            var auth = new Authenticator(LoginCallback);
            auth.ServerError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Server error:", MessageBoxButton.OK);
                //logError(errorMessage);
            };
            auth.UserError += (s, errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Login error:", MessageBoxButton.OK);
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

            return new Authenticator.Response
            {
                IsGivingUp = false,
                Username = login.ViewModel.Username,
                Password = login.PasswordTxt.Password,
                Server = login.ViewModel.Server,
                Port = login.ViewModel.Port
            };
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _core?.Dispose();
        }
    }
}