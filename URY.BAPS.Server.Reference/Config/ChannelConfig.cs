using Microsoft.Extensions.Logging;

namespace URY.BAPS.Server.Reference.Config
{
    public class ChannelConfig : ConfigBase<ChannelConfig>
    {
        public ChannelConfig(byte id, ILogger<ChannelConfig> logger) : base(logger)
        {
            Id = id;
            Name = $"Channel {id + 1}";
        }
        
        public byte Id { get; }
        
        public string Name { get; set; }

        public override void DumpToLogger()
        {
            Logger.LogInformation("Channel #{Id}: name {Name}", Id, Name);
        }
    }
}