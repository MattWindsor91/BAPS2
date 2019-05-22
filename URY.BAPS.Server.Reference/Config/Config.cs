using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace URY.BAPS.Server.Reference.Config
{
    /// <summary>
    ///     Base interface of all BAPS config tree nodes.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        ///     Dumps this config node to its attached logger.
        /// </summary>
        void DumpToLogger();
    }

    public abstract class ConfigBase<T> : IConfig
    {
        protected ConfigBase(ILogger<T>? logger)
        {
            Logger = logger ?? NullLogger<T>.Instance;
        }

        protected ILogger<T> Logger { get; }

        public abstract void DumpToLogger();
    }

    /// <summary>
    ///     Root of the BAPS server config tree.
    /// </summary>
    public class ServerConfig : ConfigBase<ServerConfig>
    {

        public ServerConfig(ListenConfig listen, ChannelSetConfig channelSet, ILogger<ServerConfig>? logger) : base(logger)
        {
            Listen = listen;
            ChannelSet = channelSet;
        }

        public ListenConfig Listen { get; }
        public ChannelSetConfig ChannelSet { get; }

        public override void DumpToLogger()
        {
            Logger.LogInformation("Here comes the BAPS server configuration.");
            Listen.DumpToLogger();
            ChannelSet.DumpToLogger();
        }
    }
}