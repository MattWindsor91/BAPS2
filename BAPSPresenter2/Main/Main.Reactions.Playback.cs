// This used to be BAPSPresenterMainReactions_player.cpp.

using System;
using BAPSCommon;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void showChannelOperation(ushort channelID, Command operation)
        {
            if (ChannelOutOfBounds(channelID)) return;
            var chan = bapsChannels[channelID];
            switch (operation)
            {
                case Command.PLAY:
                    chan.ShowPlay();
                    timeLine.locked[channelID] = true;
                    break;
                case Command.PAUSE:
                    chan.ShowPause();
                    timeLine.locked[channelID] = false;
                    break;
                case Command.STOP:
                    chan.ShowStop();
                    timeLine.locked[channelID] = false;
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
            refreshAudioWall();
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