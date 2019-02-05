using BAPSClientWindows;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace BAPSPresenterNG.ViewModel
{
    [UsedImplicitly]
    public class LoginViewModel : ViewModelBase
    {
        private int _port;
        [NotNull] private string _server;
        [NotNull] private string _username;

        public LoginViewModel(ConfigManager configManager)
        {
            _server = configManager.GetValue("ServerAddress", "localhost");

            int.TryParse(configManager.GetValue("ServerPort", "1350"), out var temp);
            _port = temp;

            _username = configManager.GetValue("DefaultUsername", "");
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