using System;
using URY.BAPS.Common.Protocol.V2.Ops;

using static URY.BAPS.Common.Protocol.V2.Commands.CommandUnpacking;

namespace URY.BAPS.Common.Protocol.V2.Commands
{

    /// <summary>
    ///     Builds unpacked <see cref="ICommand" />s from packed command words.
    /// </summary>
    public static class CommandFactory
    {
        private static ICommand UnpackConfig(ushort word)
        {
            var op = ConfigOp(word);
            var modeFlag = HasModeFlag(word);

            // Both config indices and config values only take up six bytes,
            // so we use the same mask.
            return op.CanTakeIndex()
                ? UnpackIndexableConfig(op, modeFlag, word)
                : new ValueConfigCommand(op, ConfigIndex(word), modeFlag);
        }

        private static ICommand UnpackIndexableConfig(ConfigOp op, bool modeFlag, ushort word)
        {
            var indexedFlag = HasConfigIndexedFlag(word);
            return indexedFlag
                ? (ICommand) new IndexedConfigCommand(op, ConfigIndex(word), modeFlag)
                : new NonIndexedConfigCommand(op, modeFlag);
        }

        private static ICommand UnpackDatabase(ushort word)
        {
            return new DatabaseCommand(DatabaseOp(word), Value(word), HasModeFlag(word));
        }

        private static ICommand UnpackPlayback(ushort word)
        {
            return new PlaybackCommand(PlaybackOp(word), Channel(word), HasChannelModeFlag(word));
        }

        private static ICommand UnpackPlaylist(ushort word)
        {
            return new PlaylistCommand(PlaylistOp(word), Channel(word), HasChannelModeFlag(word));
        }

        private static ICommand UnpackSystem(ushort word)
        {
            return new SystemCommand(SystemOp(word), Value(word), HasModeFlag(word));
        }

        /// <summary>
        ///     Unpacks a command word into an <see cref="ICommand" />.
        /// </summary>
        /// <param name="word">The word to unpack.</param>
        /// <returns>An <see cref="ICommand" /> whose contents match those of the packed word <paramref name="word" />.</returns>
        public static ICommand Unpack(ushort word)
        {
            return Group(word) switch
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