using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Wpf.Services;

namespace URY.BAPS.Client.Wpf.ViewModel
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

        [ProvidesContext] public static LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();

        [ProvidesContext] public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        [ProvidesContext]
        public static ConfigController ConfigController => ServiceLocator.Current.GetInstance<ConfigController>();

        [ProvidesContext]
        public static SystemController SystemController => ServiceLocator.Current.GetInstance<SystemController>();

        [ProvidesContext]
        public static ChannelControllerSet ChannelControllerSet =>
            ServiceLocator.Current.GetInstance<ChannelControllerSet>();

        [ProvidesContext]
        public static DirectoryControllerSet DirectoryControllerSet =>
            ServiceLocator.Current.GetInstance<DirectoryControllerSet>();

        private static void RegisterServices()
        {
            SimpleIoc.Default.Register<ConfigCache>();
            SimpleIoc.Default.Register<ConfigController>();
            SimpleIoc.Default.Register<SystemController>();
            SimpleIoc.Default.Register<ChannelControllerSet>();
            SimpleIoc.Default.Register<DirectoryControllerSet>();
            SimpleIoc.Default.Register<AudioWallService>();
        }
    }
}