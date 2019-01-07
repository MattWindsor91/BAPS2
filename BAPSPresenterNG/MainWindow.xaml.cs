using BAPSCommon;
using System.Windows;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ViewModel.MainViewModel ViewModel => DataContext as ViewModel.MainViewModel;
    }
}
