using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Common.ClientConfig;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     Normal implementation of a <see cref="ILoginViewModel"/>.
    /// </summary>
    [UsedImplicitly]
    public class LoginViewModel : ViewModelBase, ILoginViewModel
    {
        [NotNull] private string _username;

        /// <summary>
        ///     Constructs a <see cref="LoginViewModel"/>, retrieving all
        ///     default server information from the client config.
        /// </summary>
        /// <param name="configManager">
        ///     The <see cref="IClientConfigManager"/> from which all default
        ///     server information will be read.
        /// </param>
        public LoginViewModel(IClientConfigManager configManager)
        {
            var config = configManager.LoadConfig();
            _username = config.DefaultUsername;
        }

        [NotNull]
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }
    }
}