// This used to be BAPSPresenterMainReactions_playlist.cpp.

using BAPSClientCommon;
using System;
using BAPSClientCommon.Events;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupPlaylistReactions(IServerUpdater r)
        {
            r.ObserveTrackAdd += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.TrackAddEventArgs>)AddItem, sender, e);
                }
                else AddItem(sender, e);
            };
            r.ObserveTrackMove += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.TrackMoveEventArgs>)MoveItemTo, sender, e);
                }
                else MoveItemTo(sender, e);
            };
            r.ObserveTrackDelete += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.TrackDeleteEventArgs>)DeleteItem, sender, e);
                }
                else DeleteItem(sender, e);
            };
            r.ObservePlaylistReset += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.PlaylistResetEventArgs>)CleanPlaylist, sender, e);
                }
                else CleanPlaylist(sender, e);
            };
        }

        private void AddItem(object sender, Updates.TrackAddEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            _channels[e.ChannelId].AddTrack(e.Index, e.Item);
            RefreshAudioWall();
        }

        private void MoveItemTo(object sender, Updates.TrackMoveEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            _channels[e.ChannelId].MoveTrack(e.Index, e.NewIndex);
            RefreshAudioWall();
        }

        private void DeleteItem(object sender, Updates.TrackDeleteEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            _channels[e.ChannelId].RemoveTrack(e.Index);
            RefreshAudioWall();
        }

        private void CleanPlaylist(object sender, Updates.PlaylistResetEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            _channels[e.ChannelId].CleanPlaylist();
            RefreshAudioWall();
        }
    }
}