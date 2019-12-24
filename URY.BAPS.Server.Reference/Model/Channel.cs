using System;
using URY.BAPS.Server.Config;

namespace URY.BAPS.Server.Model
{
    public class Channel
    {
        public Channel(ChannelConfig config, Func<byte, Playlist> playlistFactory, Player player)
        {
            _config = config;
            _playlist = playlistFactory(Id);
            _player = player;
        }

        public byte Id => _config.Id;

        private Playlist _playlist;
        private Player _player;
        private readonly ChannelConfig _config;
    }
}