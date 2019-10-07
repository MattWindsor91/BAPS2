using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Normal implementation of a <see cref="ILoginViewModel"/>.
    /// </summary>
    [UsedImplicitly]
    public class LoginViewModel : ReactiveObject, ILoginViewModel
    {
        private int _port;
        [NotNull] private string _server;
        [NotNull] private string _username;

        /// <summary>
        ///     Constructs a <see cref="LoginViewModel"/>, retrieving all
        ///     default server information from the client config.
        /// </summary>
        /// <param name="configManager">
        ///     The <see cref="IClientConfigManager"/> from which all default
        ///     server information will be read.
        /// </param>
        public LoginViewModel(IClientConfigManager configManager)
        {
            var config = configManager.LoadConfig();
            _server = config.ServerAddress;
            _port = config.ServerPort;
            _username = config.DefaultUsername;
        }

        [NotNull]
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        public int Port
        {
            get => _port;
            set => this.RaiseAndSetIfChanged(ref _port, value);
        }
    }
}