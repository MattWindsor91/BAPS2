using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace BAPSClientCommon.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="ChannelController" />s.
    /// </summary>
    public class ChannelControllerSet
    {
        [NotNull] private readonly ConfigController _config;

        [NotNull] private readonly ConcurrentDictionary<ushort, ChannelController> _controllers =
            new ConcurrentDictionary<ushort, ChannelController>();

        [NotNull] private readonly ClientCore _coreForMakingChannels;

        public ChannelControllerSet([NotNull] ClientCore core, [NotNull] ConfigController config)
        {
            _coreForMakingChannels = core;
            _config = config;
        }

        /// <summary>
        ///     Gets a controller for a channel with the given ID.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns>
        ///     A <see cref="ChannelController" /> for <paramref name="channelId" />.
        ///     Any controllers this set previously constructed for the given ID will be reused.
        /// </returns>
        [NotNull]
        public ChannelController ControllerFor(ushort channelId)
        {
            return _controllers.GetOrAdd(channelId, MakeChannelController);
        }

        [NotNull]
        [Pure]
        private ChannelController MakeChannelController(ushort channelId)
        {
            return new ChannelController(channelId, _coreForMakingChannels, _config);
        }
    }
}