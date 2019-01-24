﻿// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSClientCommon;
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
                    Invoke((ServerUpdates.ItemAddEventHandler)AddItem, sender, e);
                }
                else AddItem(sender, e);
            };
            r.ItemMove += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((ServerUpdates.ItemMoveEventHandler)MoveItemTo, sender, e);
                }
                else MoveItemTo(sender, e);
            };
            r.ItemDelete += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((ServerUpdates.ItemDeleteEventHandler)DeleteItem, sender, e);
                }
                else DeleteItem(sender, e);
            };
            r.ResetPlaylist += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((ServerUpdates.ChannelResetEventHandler)CleanPlaylist, sender, e);
                }
                else CleanPlaylist(sender, e);
            };
        }

        private void AddItem(object sender, ServerUpdates.ItemAddEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            bapsChannels[e.ChannelId].AddTrack(e.Index, e.Item);
            RefreshAudioWall();
        }

        private void MoveItemTo(object sender, ServerUpdates.ItemMoveEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            bapsChannels[e.ChannelId].MoveTrack(e.Index, e.NewIndex);
            RefreshAudioWall();
        }

        private void DeleteItem(object sender, ServerUpdates.ItemDeleteEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            bapsChannels[e.ChannelId].RemoveTrack(e.Index);
            RefreshAudioWall();
        }

        private void CleanPlaylist(object sender, ServerUpdates.ChannelResetEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            bapsChannels[e.ChannelId].CleanPlaylist();
            RefreshAudioWall();
        }
    }
}