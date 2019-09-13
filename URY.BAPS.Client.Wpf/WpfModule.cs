using Autofac;
using URY.BAPS.Client.Common.Auth;
using URY.BAPS.Client.Common.Auth.Prompt;
using URY.BAPS.Client.Wpf.Auth;
using URY.BAPS.Client.Wpf.Services;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf
{
    /// <summary>
    ///     Autofac module containing the WPF-specific components.
    /// </summary>
    public class WpfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterServices(builder);
            RegisterViewModels(builder);
            RegisterRest(builder);
        }

        /// <summary>
        ///     Registers the services used by the BAPS client.
        /// </summary>
        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<AudioWallService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ChannelFactoryService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DirectoryFactoryService>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Registers the view models used by the BAPS client.
        /// </summary>
        private static void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<TextViewModel>().As<ITextViewModel>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
        }

        /// <summary>
        ///     Registers parts of the BAPS client that don't easily fit into
        ///     the above categories.
        /// </summary>
        private static void RegisterRest(ContainerBuilder builder)
        {
            builder.Register(c => new MainWindow { DataContext = c.Resolve<MainViewModel>() }).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DialogLoginPrompter>().As<ILoginPrompter>().InstancePerLifetimeScope();
            builder.RegisterType<MessageBoxLoginErrorHandler>().As<ILoginErrorHandler>().InstancePerLifetimeScope();
        }
    }
}