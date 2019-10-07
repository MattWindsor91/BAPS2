using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.ViewModel;
using URY.BAPS.Client.Wpf.Dialogs;

namespace URY.BAPS.Client.Wpf.Auth
{
    /// <summary>
    ///     An <see cref="ILoginPrompter"/> that uses the
    ///     <see cref="Login"/> dialog to request loginPrompt details.
    /// </summary>
    public class DialogLoginPrompter : ILoginPrompter
    {
        private readonly MainWindow _parent;
        private readonly LoginViewModel _viewModel;

        /// <summary>
        ///     Constructs a <see cref="DialogLoginPrompter"/>.
        /// </summary>
        /// <param name="parent">
        ///     The parent window, used to position the login dialogs.
        /// </param>
        /// <param name="viewModel">
        ///     The view model, used to store non-sensitive parts of the
        ///     login data.
        /// </param>
        public DialogLoginPrompter(MainWindow parent, LoginViewModel viewModel)
        {
            _parent = parent;
            _viewModel = viewModel;
        }

        public void Prompt()
        {
            var login = new Login
            {
                Owner = _parent,
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