using BAPSClientWindows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace BAPSPresenterNG.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private int _port;
        private string _server;
        private string _username;

        public LoginViewModel()
        {
            var configManager = SimpleIoc.Default.GetInstance<ConfigManager>();
            Server = configManager.GetValue("ServerAddress", "localhost");

            int.TryParse(configManager.GetValue("ServerPort", "1350"), out var temp);
            Port = temp;

            Username = configManager.GetValue("DefaultUsername", "");
        }

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