using System;
using JetBrains.Annotations;

namespace URY.BAPS.Client.Common.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="ChannelController" />s.
    /// </summary>
    public class ChannelControllerSet : ControllerSetBase<ChannelController>
    {
        [NotNull] private readonly ConfigController _config;

        public ChannelControllerSet([CanBeNull] IClientCore core, [CanBeNull] ConfigController config) : base(core)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [Pure]
        protected override ChannelController MakeController(ushort channelId)
        {
            return new ChannelController(channelId, Core, _config);
        }
    }
}