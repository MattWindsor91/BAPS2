using System.Collections.Generic;
using System.Collections.Immutable;

namespace URY.BAPS.Server.Reference.Model
{
    public class ChannelSet
    {
        private readonly ImmutableArray<Channel> _channels;
        
        public ChannelSet(IEnumerable<Channel> channels)
        {
            _channels = channels.ToImmutableArray();
        }
    }
}