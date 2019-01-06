using System.Windows.Controls;
using System.Windows.Input;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for Channel.xaml
    /// </summary>
    public partial class Channel : UserControl
    {
        public Channel()
        {
            InitializeComponent();
        }

        private void PlayCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(DataContext is ChannelViewModel viewModel)) return;
            e.CanExecute = !viewModel.IsPlaying;
        }

        private void PauseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PlayCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(DataContext is ChannelViewModel viewModel)) return;
            viewModel.Controller?.Play();
        }

        private void PauseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(DataContext is ChannelViewModel viewModel)) return;
            viewModel.Controller?.Pause();
        }

        private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(DataContext is ChannelViewModel viewModel)) return;
            viewModel.Controller?.Stop();
        }
    }
}
