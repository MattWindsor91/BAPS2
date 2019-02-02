using System.Windows;
using BAPSPresenterNG.ViewModel;

namespace BAPSPresenterNG
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Exposes the login view model.
        /// </summary>
        public LoginViewModel ViewModel => DataContext as LoginViewModel;

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}