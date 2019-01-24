// This used to be BAPSPresenterMainReactions_playback.cpp.

using System;
using BAPSCommon;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupPlaybackReactions(Receiver r)
        {
            r.ChannelState += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((ServerUpdates.ChannelStateEventHandler)showChannelOperation, sender, e);
                }
                else showChannelOperation(sender, e);
            };
            r.Position += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, PositionType, uint>)ShowPositionWithType, e.channelID, e.type, e.position);
                }
                else ShowPositionWithType(e.channelID, e.type, e.position);
            };
            r.Duration += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint>)showDuration, e.channelID, e.duration);
                }
                else showDuration(e.channelID, e.duration);
            };
            r.LoadedItem += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, TracklistItem>)showLoadedItem, e.channelID, e.index, e.entry);
                }
                else showLoadedItem(e.channelID, e.index, e.entry);
            };
            r.TextItem += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, TextTracklistItem>)showText, e.ChannelID, e.index, e.entry);
                }
                else showText(e.ChannelID, e.index, e.entry);
            };
        }

        private void ShowPositionWithType(ushort channelID, PositionType type, uint position)
        {
            switch (type)
            {
                case PositionType.Cue:
                    showCuePosition(channelID, position);
                    break;
                case PositionType.Intro:
                    showIntroPosition(channelID, position);
                    break;
                case PositionType.Position:
                    showPosition(channelID, position);
                    break;
            }
        }

        private void showChannelOperation(object sender, ServerUpdates.ChannelStateEventArgs e)
        {
            if (ChannelOutOfBounds(e.ChannelId)) return;
            var chan = bapsChannels[e.ChannelId];
            switch (e.State)
            {
                case ChannelState.Playing:
                    chan.ShowPlay();
                    timeLine.Lock(e.ChannelId);
                    break;
                case ChannelState.Paused:
                    chan.ShowPause();
                    timeLine.Unlock(e.ChannelId);
                    break;
                case ChannelState.Stopped:
                    chan.ShowStop();
                    timeLine.Unlock(e.ChannelId);
                    break;
            }
        }

        private void showPosition(ushort channelID, uint value)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedPosition = (int)value;
        }

        private void showLoadedItem(ushort channelID, uint index, TracklistItem entry)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].ShowLoadedItem(index, entry);
            RefreshAudioWall();
        }

        private void showDuration(ushort channelID, uint value)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedDuration = (int)value;
        }

        private void showText(ushort channel, uint index, TextTracklistItem entry)
        {
            if (ChannelOutOfBounds(channel)) return;
            foreach (var chan in bapsChannels) chan.LoadedTextIndex = -1;
            bapsChannels[channel].LoadedTextIndex = (int)index;
            MainTextDisplay.Text = entry.Text;
            if (textDialog?.Visible ?? false)
            {
                textDialog.Invoke((Action<string>)textDialog.updateText, entry.Text);
            }
        }

        private void showCuePosition(ushort channelID, uint cuePosition)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedCuePosition = (int)cuePosition;
        }

        private void showIntroPosition(ushort channelID, uint introPosition)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedIntroPosition = (int)introPosition;
        }
    }
}