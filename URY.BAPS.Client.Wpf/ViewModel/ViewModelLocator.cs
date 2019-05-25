using System;
using JetBrains.Annotations;


namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     This class contains references to the various 'main' view models
    ///     used in the presenter, allowing them to be resolved from within
    ///     their corresponding view XAML files.
    /// </summary>
    public class ViewModelLocator
    {
        private readonly Lazy<LoginViewModel> _login;
        private readonly Lazy<MainViewModel> _main;

        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator(Lazy<LoginViewModel> login, Lazy<MainViewModel> main)
        {
            _login = login;
            _main = main;
        }

     [ProvidesContext] public LoginViewModel Login => _login.Value;

     [ProvidesContext] public MainViewModel Main => _main.Value;
    }
}