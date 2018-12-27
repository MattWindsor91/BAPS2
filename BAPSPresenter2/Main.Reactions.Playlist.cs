// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void addItem(ushort channel, uint index, uint type, string description)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].AddTrack(index, type, description);
            refreshAudioWall();
        }

        private void moveItemTo(ushort channel, uint oldIndex, uint newIndex)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].MoveTrack(oldIndex, newIndex);
            refreshAudioWall();
        }

        private void deleteItem(ushort channel, uint index)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].RemoveTrack(index);
            refreshAudioWall();
        }

        private void cleanPlaylist(ushort channel)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].CleanPlaylist();
            refreshAudioWall();
        }
    }
}