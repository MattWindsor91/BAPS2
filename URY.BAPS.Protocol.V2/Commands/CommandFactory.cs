using System;

namespace URY.BAPS.Protocol.V2.Commands
{
    /// <summary>
    ///     Builds unpacked <see cref="ICommand"/>s from <see cref="CommandWord"/>s.
    /// </summary>
    public static class CommandFactory
    {
        private static ICommand UnpackConfig(CommandWord word)
        {
            var op = word.ConfigOp();
            return word.HasConfigIndexedFlag()
                ? (ICommand) new IndexedConfigCommand(op, word.ConfigIndex())
                : new ConfigCommand(op);
        }

        private static ICommand UnpackDatabase(CommandWord word)
        {
            return new DatabaseCommand(word.DatabaseOp(), word.Value(), word.HasModeFlag());
        }

        private static ICommand UnpackPlayback(CommandWord word)
        {
            return new PlaybackCommand(word.PlaybackOp(), word.Channel(), word.HasChannelModeFlag());
        }

        private static ICommand UnpackPlaylist(CommandWord word)
        {
            return new PlaylistCommand(word.PlaylistOp(), word.Channel(), word.HasChannelModeFlag());
        }

        private static ICommand UnpackSystem(CommandWord word)
        {
            return new SystemCommand(word.SystemOp(), word.Value(), word.HasModeFlag());
        }

        /// <summary>
        ///     Unpacks a command word into an <see cref="ICommand"/>.
        /// </summary>
        /// <param name="word">The word to unpack.</param>
        /// <returns>An <see cref="ICommand"/> whose contents match those of the packed word <paramref name="word"/>.</returns>
        public static ICommand Unpack(this CommandWord word)
        {
            return word.Group() switch
            {
                 CommandGroup.Config => UnpackConfig(word),
                 CommandGroup.Database => UnpackDatabase(word),
                 CommandGroup.Playback => UnpackPlayback(word),
                 CommandGroup.Playlist => UnpackPlaylist(word),
                 CommandGroup.System => UnpackSystem(word),
                 _ => throw new ArgumentOutOfRangeException(nameof(word), word, "Invalid command word group")
            };
        }
    }
}