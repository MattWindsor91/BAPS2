// This used to be BAPSPresenterMainReactions_playback.cpp.

using System;
using BAPSClientCommon;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupPlaybackReactions(IServerUpdater r)
        {
            r.ObservePlayerState += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.PlayerStateEventArgs>)ShowChannelOperation, sender, e);
                }
                else ShowChannelOperation(sender, e);
            };
            r.ObserveMarker += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.MarkerEventArgs>)ShowPositionWithType, sender, e);
                }
                else ShowPositionWithType(sender, e);
            };
            r.ObserveTrackLoad += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.TrackLoadEventArgs>)ShowLoadedItem, this, e);
                }
                else ShowLoadedItem(this, e);
            };
        }

        private void ShowPositionWithType(object sender, Updates.MarkerEventArgs e)
        {
            switch (e.Marker)
            {
                case MarkerType.Cue:
                    ShowCuePosition(e.ChannelId, e.NewValue);
                    break;
                case MarkerType.Intro:
                    ShowIntroPosition(e.ChannelId, e.NewValue);
                    break;
                case MarkerType.Position:
                    ShowPosition(e.ChannelId, e.NewValue);
                    break;
            }
        }

        private void ShowChannelOperation(object sender, Updates.PlayerStateEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            var chan = _channels[e.ChannelId];
            switch (e.State)
            {
                case PlaybackState.Playing:
                    chan.ShowPlay();
                    timeLine.Lock(e.ChannelId);
                    break;
                case PlaybackState.Paused:
                    chan.ShowPause();
                    timeLine.Unlock(e.ChannelId);
                    break;
                case PlaybackState.Stopped:
                    chan.ShowStop();
                    timeLine.Unlock(e.ChannelId);
                    break;
            }
        }

        private void ShowPosition(ushort channelId, uint value)
        {
            if (ChannelOutOfBounds(channelId)) return;
            _channels[channelId].DisplayedPosition = (int)value;
        }

        private void ShowLoadedItem(object sender, Updates.TrackLoadEventArgs args)
        {
            if (ChannelOutOfBounds(args.ChannelId)) return;
            var channel = _channels[args.ChannelId];

            var track = args.Track;
            if (track.IsTextItem)
            {
                ShowText(args.ChannelId, args.Index, track);
            }
            else
            {
                channel.ShowLoadedItem(args.Index, args.Track);
                channel.DisplayedDuration = (int)args.Track.Duration;
                RefreshAudioWall();
            }
        }

        private void ShowText(ushort channel, uint index, Track entry)
        {
            if (ChannelOutOfBounds(channel)) return;
            foreach (var chan in _channels) chan.LoadedTextIndex = -1;
            _channels[channel].LoadedTextIndex = (int)index;
            MainTextDisplay.Text = entry.Text;
            if (textDialog?.Visible ?? false)
            {
                textDialog.Invoke((Action<string>)textDialog.updateText, entry.Text);
            }
        }

        private void ShowCuePosition(ushort channelId, uint cuePosition)
        {
            if (ChannelOutOfBounds(channelId)) return;
            _channels[channelId].DisplayedCuePosition = (int)cuePosition;
        }

        private void ShowIntroPosition(ushort channelId, uint introPosition)
        {
            if (ChannelOutOfBounds(channelId)) return;
            _channels[channelId].DisplayedIntroPosition = (int)introPosition;
        }
    }
}