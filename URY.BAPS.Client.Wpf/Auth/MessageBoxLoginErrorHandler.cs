using System;
using System.Windows;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.LoginResult;

namespace URY.BAPS.Client.Wpf.Auth
{
    /// <summary>
    ///     A <see cref="ILoginErrorHandler"/> that responds to loginPrompt
    ///     errors by opening WPF message boxes.
    /// </summary>
    class MessageBoxLoginErrorHandler : ILoginErrorHandler
    {
        /// <summary>
        ///     The main window, used as the parent of any loginPrompt error dialogs.
        /// </summary>
        private readonly MainWindow _mainWindow;

        public MessageBoxLoginErrorHandler(MainWindow? mainWindow)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        public void Handle(ILoginResult result)
        {
            if (result.IsSuccess) return;
            if (result.IsUserFault)
            {
                OnUserError(result.Description);
            }
            else
            {
                OnServerError(result.Description);
            }
        }

        public void OnServerError(string details)
        {
            ShowErrorDialog("Server error", details);
        }

        public void OnUserError(string details)
        {
            ShowErrorDialog("User error", details);
        }

        private void ShowErrorDialog(string caption, string details)
        {
            MessageBox.Show(_mainWindow, details, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
