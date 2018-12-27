// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        void addItem(ushort channel, uint index, uint type, string description)
        {
            if (3 <= channel) return;
            /** Add an item to the end of the list ( only method currently supported by server ) **/
            trackList[channel].addTrack((int)type, description);
            refreshAudioWall();
        }

        void moveItemTo(ushort channel, uint oldIndex, uint newIndex)
        {
            if (3 <= channel) return;
            trackList[channel].moveTrack((int)oldIndex, (int)newIndex);
            refreshAudioWall();
        }

        void deleteItem(ushort channel, uint index)
        {
            if (3 <= channel) return;
            trackList[channel].removeTrack((int)index);
            refreshAudioWall();
        }

        void cleanPlaylist(ushort channel)
        {
            if (3 <= channel) return;
            trackList[channel].clearTrackList();
            var cds = (CountDownState)trackLengthText[channel].Tag;
            cds.running = false;
            refreshAudioWall();
        }
    }
}