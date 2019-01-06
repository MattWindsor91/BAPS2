using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private int ChannelIDOfParameter(object parameter)
        {
            if (parameter == null) return -1;
            switch (parameter)
            {
                case int i:
                    return i;
                case ushort u:
                    return u;
                case string s:
                    {
                        var id = -1;
                        int.TryParse(s, out id);
                        return id;
                    }
                default:
                    return -1;
            }
        }

        private ChannelViewModel ChannelOfParameter(object parameter)
        {
            var channelID = ChannelIDOfParameter(parameter);
            return ViewModel.Channels.ElementAtOrDefault(channelID);
        }

        #region Command handlers for the F-key channel operation shortcuts

        private void PlayShortcut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            e.CanExecute = !(channel?.IsPlaying ?? true);
        }

        private void PlayShortcut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            channel?.Controller?.Play();
        }

        private void PauseShortcut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            e.CanExecute = channel != null;
        }

        private void PauseShortcut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            channel?.Controller?.Pause();
        }

        private void StopShortcut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            e.CanExecute = channel != null;
        }

        private void StopShortcut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var channel = ChannelOfParameter(e.Parameter);
            channel?.Controller?.Stop();
        }

        #endregion Command handlers for the F-key channel operation shortcuts
    }
}
