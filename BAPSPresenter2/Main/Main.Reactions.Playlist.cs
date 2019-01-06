// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSCommon;
using System;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupPlaylistReactions(Receiver r)
        {
            r.ItemAdd += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, EntryInfo>)addItem, e.channelID, e.index, e.entry);
                }
                else addItem(e.channelID, e.index, e.entry);
            };
            r.ItemMove += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, uint>)moveItemTo, e.channelID, e.indexFrom, e.indexTo);
                }
                else moveItemTo(e.channelID, e.indexFrom, e.indexTo);
            };
            r.ItemDelete += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint>)deleteItem, e.channelID, e.index);
                }
                else deleteItem(e.channelID, e.index);
            };
            r.ResetPlaylist += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort>)cleanPlaylist, e);
                }
                else cleanPlaylist(e);
            };
        }

        private void addItem(ushort channel, uint index, EntryInfo entry)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].AddTrack(index, entry);
            RefreshAudioWall();
        }

        private void moveItemTo(ushort channel, uint oldIndex, uint newIndex)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].MoveTrack(oldIndex, newIndex);
            RefreshAudioWall();
        }

        private void deleteItem(ushort channel, uint index)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].RemoveTrack(index);
            RefreshAudioWall();
        }

        private void cleanPlaylist(ushort channel)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].CleanPlaylist();
            RefreshAudioWall();
        }
    }
}