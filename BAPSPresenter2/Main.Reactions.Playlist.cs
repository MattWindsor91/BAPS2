// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void addItem(ushort channel, uint index, uint type, string description)
        {
            if (ChannelOutOfBounds(channel)) return;
            /** Add an item to the end of the list ( only method currently supported by server ) **/
            trackList[channel].addTrack((int)type, description);
            refreshAudioWall();
        }

        private void moveItemTo(ushort channel, uint oldIndex, uint newIndex)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackList[channel].moveTrack((int)oldIndex, (int)newIndex);
            refreshAudioWall();
        }

        private void deleteItem(ushort channel, uint index)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackList[channel].removeTrack((int)index);
            refreshAudioWall();
        }

        private void cleanPlaylist(ushort channel)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackList[channel].clearTrackList();
            var cds = (CountDownState)trackLengthText[channel].Tag;
            cds.running = false;
            refreshAudioWall();
        }
    }
}