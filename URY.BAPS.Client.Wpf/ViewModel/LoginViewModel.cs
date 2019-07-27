using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    [UsedImplicitly]
    public class LoginViewModel : ViewModelBase
    {
        private int _port;
        [NotNull] private string _server;
        [NotNull] private string _username;

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
            set
            {
                if (_username == value) return;
                _username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }

        public string Server
        {
            get => _server;
            set
            {
                if (_server == value) return;
                _server = value;
                RaisePropertyChanged(nameof(Server));
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (_port == value) return;
                _port = value;
                RaisePropertyChanged(nameof(Port));
            }
        }
    }
}