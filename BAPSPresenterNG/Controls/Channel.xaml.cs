using System.Windows.Controls;

namespace BAPSPresenterNG.Controls
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

        private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO(@MattWindsor91): MVVM?
            if (!(DataContext is ViewModel.ChannelViewModel viewModel)) return;
            if (!(sender is ListBox box)) return;

            var index = box.SelectedIndex;
            if (index < 0) return;
            // BAPS doesn't really support the notion of deselecting.
            if (!viewModel.IsLoadPossible(index)) return;

            var uindex = (uint)index;
            viewModel.Controller?.Select(uindex);
        }
    }
}
