using System.Windows.Controls;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Controls
{
    /// <summary>
    ///     Interaction logic for Channel.xaml
    /// </summary>
    public partial class Channel
    {
        public Channel()
        {
            InitializeComponent();
        }

        private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO(@MattWindsor91): MVVM?
            if (!(DataContext is ChannelViewModel viewModel)) return;
            if (!(sender is ListBox box)) return;

            var index = box.SelectedIndex;
            if (index < 0) return;
            // BAPS doesn't really support the notion of deselecting.
            if (!viewModel.IsLoadPossible(index)) return;

            var uindex = (uint) index;
            viewModel.Controller?.Select(uindex);
        }
    }
}