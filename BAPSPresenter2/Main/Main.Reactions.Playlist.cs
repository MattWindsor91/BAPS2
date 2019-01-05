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
                    Invoke((Action<ushort, uint, uint, string>)addItem, e.channelID, e.index, e.type, e.description);
                }
                else addItem(e.channelID, e.index, e.type, e.description);
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

        private void addItem(ushort channel, uint index, uint type, string description)
        {
            if (ChannelOutOfBounds(channel)) return;
            bapsChannels[channel].AddTrack(index, type, description);
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