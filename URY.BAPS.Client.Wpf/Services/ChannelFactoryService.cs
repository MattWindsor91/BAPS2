using System;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.ViewModel;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.Services
{
    /// <summary>
    ///     A service that builds <see cref="IChannelViewModel" />s and their dependencies.
    /// </summary>
    [UsedImplicitly]
    public class ChannelFactoryService
    {
        [NotNull] private readonly AudioWallService _audioWallService;
        [NotNull] private readonly ConfigCache _config;
        [NotNull] private readonly ChannelControllerSet _controllerSet;

        /// <summary>
        ///     Constructs a channel factory.
        /// </summary>
        /// <param name="controllerSet">
        ///     The controller set from which we get controllers for each channel (to talk to the BAPS server).
        /// </param>
        /// <param name="audioWallService">
        ///     The audio wall service that the channel view models should use to request an audio wall.
        /// </param>
        /// <param name="config">
        ///     The config cache used to check for channel configuration and related updates.
        /// </param>
        public ChannelFactoryService(
            ChannelControllerSet? controllerSet,
            AudioWallService? audioWallService,
            ConfigCache? config)
        {
            _controllerSet = controllerSet ?? throw new ArgumentNullException(nameof(controllerSet));
            _audioWallService = audioWallService ?? throw new ArgumentNullException(nameof(audioWallService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        ///     Creates a channel view model.
        /// </summary>
        /// <param name="id">The ID of the channel whose view model is being created.</param>
        /// <returns>A <see cref="IChannelViewModel" /> over channel <see cref="id" />.</returns>
        [Pure]
        public IChannelViewModel Make(byte id)
        {
            var controller = _controllerSet.ControllerFor(id);
            var player = MakePlayerViewModel(controller);
            var trackList = new TrackListViewModel(id, controller);
            return new ChannelViewModel(id, _config, player, trackList, controller, _audioWallService);
        }

        private static PlayerViewModel MakePlayerViewModel(IPlaybackController controller)
        {
            var transport = new PlayerTransportViewModel(controller);
            var markers = new PlayerMarkerViewModel(controller);
            var player = new PlayerViewModel(transport, markers, controller);
            return player;
        }
    }
}