using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAPSPresenterNG
{
    /// <summary>
    /// The main BAPS presenter view model, exposing the current state
    /// of the BAPS server to the presenter.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Text = "<You can type notes here>";
        }

        public ObservableCollection<ChannelViewModel> Channels { get; } = new ObservableCollection<ChannelViewModel>();

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private string _text;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
