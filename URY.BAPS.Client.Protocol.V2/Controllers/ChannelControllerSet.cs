using System;
using JetBrains.Annotations;
using URY.BAPS.Client.Protocol.V2.Core;
using URY.BAPS.Common.Protocol.V2.MessageIo;

namespace URY.BAPS.Client.Protocol.V2.Controllers
{
    /// <summary>
    ///     Contains and constructs <see cref="ChannelController" />s.
    /// </summary>
    public class ChannelControllerSet : ControllerSetBase<ChannelController>
    {
        [NotNull] private readonly ConfigController _config;

        public ChannelControllerSet(MessageConnectionManager? core, ConfigController? config) : base(core)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [Pure]
        protected override ChannelController MakeController(byte channelId)
        {
            return new ChannelController(channelId, ConnectionManager, _config);
        }
    }
}