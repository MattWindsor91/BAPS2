// This used to be BAPSPresenterMainReactions_playback.cpp.

using System;
using BAPSCommon;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupPlaybackReactions(Receiver r)
        {
            r.ChannelOperation += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, Command>)showChannelOperation, e.channelID, e.op);
                }
                else showChannelOperation(e.channelID, e.op);
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
                    Invoke((Action<ushort, uint, Command, string>)showLoadedItem, e.channelID, e.index, e.type, e.description);
                }
                else showLoadedItem(e.channelID, e.index, e.type, e.description);
            };
            r.TextItem += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, string, string>)showText, e.ChannelID, e.index, e.description, e.text);
                }
                else showText(e.ChannelID, e.index, e.description, e.text);
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

        private void showChannelOperation(ushort channelID, Command operation)
        {
            if (ChannelOutOfBounds(channelID)) return;
            var chan = bapsChannels[channelID];
            switch (operation)
            {
                case Command.PLAY:
                    chan.ShowPlay();
                    timeLine.Lock(channelID);
                    break;
                case Command.PAUSE:
                    chan.ShowPause();
                    timeLine.Unlock(channelID);
                    break;
                case Command.STOP:
                    chan.ShowStop();
                    timeLine.Unlock(channelID);
                    break;
            }
        }

        private void showPosition(ushort channelID, uint value)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedPosition = (int)value;
        }

        private void showLoadedItem(ushort channelID, uint index, Command itemType, string description)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].ShowLoadedItem(index, itemType, description);
            RefreshAudioWall();
        }

        private void showDuration(ushort channelID, uint value)
        {
            if (ChannelOutOfBounds(channelID)) return;
            bapsChannels[channelID].DisplayedDuration = (int)value;
        }

        private void showText(ushort channel, uint index, string description, string text)
        {
            if (ChannelOutOfBounds(channel)) return;
            foreach (var chan in bapsChannels) chan.LoadedTextIndex = -1;
            bapsChannels[channel].LoadedTextIndex = (int)index;
            MainTextDisplay.Text = text;
            if (textDialog?.Visible ?? false)
            {
                textDialog.Invoke((Action<string>)textDialog.updateText, text);
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