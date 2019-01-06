using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAPSPresenterNG
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public string Username
        {
            get => _username;
            set {
                if (_username == value) return;
                _username = value;
                OnPropertyChanged(nameof(Username));
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
                OnPropertyChanged(nameof(Server));
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
                OnPropertyChanged(nameof(Port));
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
