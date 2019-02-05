using BAPSClientCommon;
using BAPSClientCommon.Controllers;
using BAPSClientCommon.ServerConfig;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

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

        [ProvidesContext]
        public static IClientCore ClientCore =>
            ServiceLocator.Current.GetInstance<IClientCore>();

        [ProvidesContext]
        public static LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        
        [ProvidesContext]
        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        [ProvidesContext]
        public static ChannelControllerSet ControllerSet => ServiceLocator.Current.GetInstance<ChannelControllerSet>();
        
        private static void RegisterServices()
        {
            SimpleIoc.Default.Register<ConfigCache>();
            SimpleIoc.Default.Register<ConfigController>();
            SimpleIoc.Default.Register<ChannelControllerSet>();
        }
    }
}