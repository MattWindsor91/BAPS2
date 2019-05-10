using URY.BAPS.Model.Track;
using URY.BAPS.Protocol.V2.Commands;
using URY.BAPS.Protocol.V2.Encode;
using URY.BAPS.Protocol.V2.Model;

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
        private static ICommand MakeAddItemCommand(byte channelId)
        {
            return new PlaylistCommand(PlaylistOp.AddItem, channelId, false);
        }

        /// <summary>
        ///     Makes the common starting base of an add-item message.
        /// </summary>
        /// <param name="channelId">The ID of the channel to which we are adding.</param>
        /// <param name="trackType">The type of track we are adding.</param>
        /// <returns>A message, to which we can add type-specific arguments.</returns>
        private static MessageBuilder MakeAddItemBase(byte channelId, TrackType trackType)
        {
            return new MessageBuilder(MakeAddItemCommand(channelId)).Add((uint)trackType);
        }

        public static MessageBuilder MakeAddFileItem(byte channelId, uint directoryNumber, string filename)
        {
            return MakeAddItemBase(channelId, TrackType.File).Add(directoryNumber).Add(filename);
        }

        public static MessageBuilder MakeAddLibraryItem(byte channelId, uint searchItemIndex)
        {
            return MakeAddItemBase(channelId, TrackType.Library).Add(searchItemIndex);
        }

        public static MessageBuilder MakeAddTextItem(byte channelId, string briefDescription, string details)
        {
            return MakeAddItemBase(channelId, TrackType.Text).Add(briefDescription).Add(details);
        }
    }
}