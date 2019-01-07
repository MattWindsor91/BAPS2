using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            // ChannelViewModels are held by the MainViewModel.
        }

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}