using System.Collections.Generic;
using System.Collections.Immutable;
using Autofac.Features.Indexed;

namespace URY.BAPS.Server.Model
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