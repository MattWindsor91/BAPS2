using System.Windows;

namespace BAPSPresenterNG
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        /// <summary>
        /// Exposes the login view model.
        /// </summary>
        public ViewModel.LoginViewModel ViewModel => DataContext as ViewModel.LoginViewModel;

        public Login()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
