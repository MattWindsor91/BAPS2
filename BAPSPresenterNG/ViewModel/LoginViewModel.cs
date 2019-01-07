using GalaSoft.MvvmLight;

namespace BAPSPresenterNG.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public string Username
        {
            get => _username;
            set {
                if (_username == value) return;
                _username = value;
                RaisePropertyChanged(nameof(Username));
            }
        }
        private string _username;

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
        private string _server;

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
        private int _port;

        public LoginViewModel()
        {
            Server = ConfigManager.getConfigValueString("ServerAddress", "localhost");

            int.TryParse(ConfigManager.getConfigValueString("ServerPort", "1350"), out var temp);
            Port = temp;

            Username = ConfigManager.getConfigValueString("DefaultUsername", "");

            ConfigManager.getConfigValueString("ServerAddress", "1350");
        }
    }
}
