using System.Collections.Generic;
using BAPSClientCommon;
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
            
            // We register ChannelViewModels later, once we know how many to expect.
        }

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        ///     Gets an enumeration of all registered channel view models.
        /// </summary>
        public IEnumerable<ChannelViewModel> Channels =>
            ServiceLocator.Current.GetAllInstances<ChannelViewModel>();

        /// <summary>
        ///     Gets the view model for the channel with the given ID.
        ///     <para>
        ///         This won't work until something has called <see cref="RegisterChannels"/>.
        ///     </para>
        /// </summary>
        /// <param name="channelId">The ID of the channel whose view model is required</param>
        /// <returns>The <see cref="ChannelViewModel"/> for the channel with ID <see cref="channelId"/>.</returns>
        public ChannelViewModel Channel(ushort channelId) =>
            ServiceLocator.Current.GetInstance<ChannelViewModel>(channelId.ToString());

        /// <summary>
        ///     Gets the view model for the player with the given channel ID.
        ///     <para>
        ///         This won't work until something has called <see cref="RegisterChannels"/>.
        ///     </para>
        /// </summary>
        /// <param name="channelId">The ID of the player whose view model is required</param>
        /// <returns>The <see cref="PlayerViewModel"/> for the channel with ID <see cref="channelId"/>.</returns>
        public PlayerViewModel Player(ushort channelId) =>
            ServiceLocator.Current.GetInstance<PlayerViewModel>(channelId.ToString());

        public ClientCore ClientCore =>
            ServiceLocator.Current.GetInstance<ClientCore>();
        
        /// <summary>
        ///     Registers view models for <paramref name="numChannels"/> channels, with IDs starting at 0.
        /// </summary>
        /// <param name="numChannels">The number of channels to register</param>
        public void RegisterChannels(ushort numChannels)
        {
            for (ushort i = 0; i < numChannels; i++)
            {
                var n = i;  // needed to make sure the right channel ID gets captured by the lambdas below.
                SimpleIoc.Default.Register<PlayerViewModel>(() => new PlayerViewModel(n), n.ToString());
                SimpleIoc.Default.Register<ChannelViewModel>(() => MakeChannelViewModel(n), n.ToString());
            }
        }

        public ChannelViewModel MakeChannelViewModel(ushort channelId)
        {
            return new ChannelViewModel(channelId, Player(channelId), ClientCore.ControllerFor(channelId));
        }
    }
}