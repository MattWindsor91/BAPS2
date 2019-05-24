using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Protocol.V2.Core;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="ChannelController" />s.
    /// </summary>
    public class ChannelControllerSet : ControllerSetBase<ChannelController>
    {
        [NotNull] private readonly ConfigController _config;

        public ChannelControllerSet(IClientCore? core, ConfigController? config) : base(core)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [Pure]
        protected override ChannelController MakeController(byte channelId)
        {
            return new ChannelController(channelId, Core, _config);
        }
    }
}