using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    public class ChannelSetConfig : ConfigBase<ChannelSetConfig>
    {
        public ChannelSetConfig(ILogger<ChannelSetConfig> logger) : base(logger)
        {
        }

        /// <summary>
        ///     The number of channels.
        /// </summary>
        public int Count { get; [UsedImplicitly] set; } = 3;

        public ImmutableArray<ChannelConfig> Channels { get; set; } =
            ImmutableArray<ChannelConfig>.Empty;

        public override void DumpToLogger()
        {
            Logger.LogInformation("Controlling {Channels} channel(s).",
                Count);
            foreach (var channel in Channels) channel.DumpToLogger();
        }
    }
}