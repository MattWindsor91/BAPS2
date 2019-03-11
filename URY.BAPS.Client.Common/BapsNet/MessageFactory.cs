using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Common.BapsNet
{
    /// <summary>
    ///     A static class containing methods for constructing valid BapsNet messages.
    /// </summary>
    public static class MessageFactory
    {
        /// <summary>
        ///     Makes the command part of an add-item message.
        /// </summary>
        /// <param name="channelId">The ID of the channel to which we are adding.</param>
        /// <returns>A command for adding an item to <paramref name="channelId"/>.</returns>
        private static Command MakeAddItemCommand(ushort channelId)
        {
            return (Command.Playlist | Command.AddItem).WithChannel(channelId);
        }

        /// <summary>
        ///     Makes the common starting base of an add-item message.
        /// </summary>
        /// <param name="channelId">The ID of the channel to which we are adding.</param>
        /// <param name="trackType">The type of track we are adding.</param>
        /// <returns>A message, to which we can add type-specific arguments.</returns>
        private static Message MakeAddItemBase(ushort channelId, TrackType trackType)
        {
            return new Message(MakeAddItemCommand(channelId)).Add((uint)trackType);
        }

        public static Message MakeAddFileItem(ushort channelId, uint directoryNumber, string filename)
        {
            return MakeAddItemBase(channelId, TrackType.File).Add(directoryNumber).Add(filename);
        }

        public static Message MakeAddLibraryItem(ushort channelId, uint searchItemIndex)
        {
            return MakeAddItemBase(channelId, TrackType.Library).Add(searchItemIndex);
        }

        public static Message MakeAddTextItem(ushort channelId, string briefDescription, string details)
        {
            return MakeAddItemBase(channelId, TrackType.Text).Add(briefDescription).Add(details);
        }
    }
}