using BAPSClientCommon;
using BAPSClientCommon.ServerConfig;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }

        public IMessenger Messenger => ServiceLocator.Current.GetInstance<IMessenger>();

        public ClientCore ClientCore =>
            ServiceLocator.Current.GetInstance<ClientCore>();

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        private void RegisterServices()
        {
            SimpleIoc.Default.Register(() => GalaSoft.MvvmLight.Messaging.Messenger.Default, true);
            SimpleIoc.Default.Register<ConfigCache>();
        }
    }
}