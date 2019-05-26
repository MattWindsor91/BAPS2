using System.Windows;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Dialogs
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Exposes the loginPrompt view model.
        /// </summary>
        public LoginViewModel? ViewModel => DataContext as LoginViewModel;

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}