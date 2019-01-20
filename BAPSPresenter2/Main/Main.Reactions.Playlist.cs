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
                    Invoke((Receiver.ItemAddEventHandler)AddItem, sender, e);
                }
                else AddItem(sender, e);
            };
            r.ItemMove += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Receiver.ItemMoveEventHandler)MoveItemTo, sender, e);
                }
                else MoveItemTo(sender, e);
            };
            r.ItemDelete += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Receiver.ItemDeleteEventHandler)DeleteItem, sender, e);
                }
                else DeleteItem(sender, e);
            };
            r.ResetPlaylist += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Receiver.ChannelResetEventHandler)CleanPlaylist, sender, e);
                }
                else CleanPlaylist(sender, e);
            };
        }

        private void AddItem(object sender, Receiver.ItemAddEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelID)) return;
            bapsChannels[e.ChannelID].AddTrack(e.Index, e.Item);
            RefreshAudioWall();
        }

        private void MoveItemTo(object sender, Receiver.ItemMoveEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelID)) return;
            bapsChannels[e.ChannelID].MoveTrack(e.Index, e.NewIndex);
            RefreshAudioWall();
        }

        private void DeleteItem(object sender, Receiver.ItemDeleteEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelID)) return;
            bapsChannels[e.ChannelID].RemoveTrack(e.Index);
            RefreshAudioWall();
        }

        private void CleanPlaylist(object sender, Receiver.ChannelResetEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelID)) return;
            bapsChannels[e.ChannelID].CleanPlaylist();
            RefreshAudioWall();
        }
    }
}