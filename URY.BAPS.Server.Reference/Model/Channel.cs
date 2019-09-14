namespace URY.BAPS.Server.Model
{
    public class Channel
    {
        public Channel(byte id, Playlist playlist, Player player)
        {
            Id = id;
            _playlist = playlist;
            _player = player;
        }
        
        public byte Id { get; }

        private Playlist _playlist;
        private Player _player;
    }
}