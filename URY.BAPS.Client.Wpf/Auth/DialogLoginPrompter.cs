using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Wpf.Dialogs;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Auth
{
    /// <summary>
    ///     A <see cref="DialogLoginPrompter"/> that uses the
    ///     <see cref="Login"/> dialog to request loginPrompt details.
    /// </summary>
    public class DialogLoginPrompter : ILoginPrompter
    {
        private readonly LoginViewModel _viewModel;

        public DialogLoginPrompter(LoginViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public MainWindow? MainWindow { get; set; }

        public void Prompt()
        {
            var login = new Login
            {
                Owner = MainWindow,
                DataContext = _viewModel
            };

            var success = login.ShowDialog() ?? false;
            // For security purposes, WPF doesn't allow a password field to
            // bind into a view model.  As such, we have to scrape it out of
            // the dialog directly.
            Response = success ? MakeLoginResponse(login.PasswordTxt.Password) : new QuitLoginPromptResponse();
        }

        private ILoginPromptResponse MakeLoginResponse(string password)
        {
            return new LoginPromptResponse(
                _viewModel.Username,
                password,
                _viewModel.Server,
                _viewModel.Port
                );
        }

        public ILoginPromptResponse Response { get; private set; } = new QuitLoginPromptResponse();
    }
}