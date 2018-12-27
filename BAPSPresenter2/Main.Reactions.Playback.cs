// This used to be BAPSPresenterMainReactions_player.cpp.

using System;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        private string TimeToString(int hours, int minutes, int seconds, int centiseconds)
        {
            /** WORK NEEDED: fix me **/
            var htemp = hours.ToString();
            var mtemp = (minutes < 10) ? string.Concat("0", minutes.ToString()) : minutes.ToString();
            var stemp = (seconds < 10) ? string.Concat("0", seconds.ToString()) : seconds.ToString();
            return string.Concat(htemp, ":", mtemp, ":", stemp);
        }

        private string MillisecondsToTimeString(int msecs)
        {
            /** WORK NEEDED: lots **/
            int secs = msecs / 1000;

            var hours = System.Math.DivRem(secs, 3600, out _);
            int mins = System.Math.DivRem(secs, 60, out _) - (hours * 60);

            secs = secs - ((mins * 60) + (hours * 3600));

            return TimeToString(hours, mins, secs, msecs % 1000 / 10);
        }

        private void showChannelOperation(ushort channel, Command operation)
        {
            switch (operation)
            {
                case Command.PLAY:
                    channelPlay[channel].BackColor = System.Drawing.Color.DarkGreen;
                    channelPlay[channel].Enabled = false;
                    timeLine.locked[channel] = true;
                    channelPause[channel].BackColor = System.Drawing.SystemColors.Control;
                    channelStop[channel].BackColor = System.Drawing.SystemColors.Control;
                    break;
                case Command.PAUSE:
                    channelPlay[channel].BackColor = System.Drawing.SystemColors.Control;
                    channelPlay[channel].Enabled = true;
                    timeLine.locked[channel] = false;
                    channelPause[channel].BackColor = System.Drawing.Color.DarkOrange;
                    channelStop[channel].BackColor = System.Drawing.SystemColors.Control;
                    break;
                case Command.STOP:
                    channelPlay[channel].BackColor = System.Drawing.SystemColors.Control;
                    channelPlay[channel].Enabled = true;
                    timeLine.locked[channel] = false;
                    channelPause[channel].BackColor = System.Drawing.SystemColors.Control;
                    channelStop[channel].BackColor = System.Drawing.Color.Firebrick;
                    break;
            }
        }

        private void showPosition(ushort channel, uint _value)
        {
            if (ChannelOutOfBounds(channel)) return;

            var value = (int)_value;

            /** Channels are ready when they have a valid duration **/
            if (trackTime[channel].Duration >= value)
            {
                /** Set the trackbar position **/
                trackTime[channel].Position = value;
                timeLine.UpdatePosition(channel, value - trackTime[channel].CuePosition);

                value = (int)(System.Math.Round(value / 1000f) * 1000);
                /** Set the amount of time gone **/
                timeGoneText[channel].Text = MillisecondsToTimeString(value);
                /** Set the time left **/
                var timeleft = trackTime[channel].Duration - value;
                timeLeftText[channel].Text = MillisecondsToTimeString(timeleft);
                if (channelPlay[channel].Enabled || timeleft > 10000 || timeleft < 500)
                {
                    nearEndTimer[channel].Enabled = false;
                    timeLeftText[channel].Highlighted = false;
                }
                else if (!channelPlay[channel].Enabled)
                {
                    nearEndTimer[channel].Interval = 100;
                    nearEndTimer[channel].Enabled = true;
                }
            }
            else
            {
                /** WORK NEEDED: there is a problem **/
            }
        }

        private void showLoadedItem(ushort channel, uint index, Command itemType, string description)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackList[channel].LoadedIndex = (int)index;
            loadedText[channel].Text = description;
            var cds = (CountDownState)trackLengthText[channel].Tag;
            cds.running = false;
            if (itemType == Command.VOIDITEM)
            {
                trackTime[channel].Position = 0;
                timeLine.UpdatePosition(channel, 0);
                trackTime[channel].Duration = 0;
                timeLine.UpdateDuration(channel, 0);
                trackTime[channel].CuePosition = 0;
                trackTime[channel].IntroPosition = 0;
                timeLeftText[channel].Text = MillisecondsToTimeString(0);
                timeGoneText[channel].Text = MillisecondsToTimeString(0);
                nearEndTimer[channel].Enabled = false;
                timeLeftText[channel].Highlighted = false;
            }
            refreshAudioWall();
        }

        private void showDuration(ushort channel, uint _value)
        {
            if (ChannelOutOfBounds(channel)) return;
            var value = (int)_value;
            trackTime[channel].Position = 0;
            trackTime[channel].Duration = value;
            timeLine.UpdateDuration(channel, value - trackTime[channel].CuePosition);
        }

        private void showText(ushort channel, uint index, string description, string text)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackList[0].LoadedTextIndex = -1;
            trackList[1].LoadedTextIndex = -1;
            trackList[2].LoadedTextIndex = -1;
            trackList[channel].LoadedTextIndex = (int)index;
            MainTextDisplay.Text = text;
            if (textDialog.Visible)
            {
                textDialog.Invoke((Action<string>)textDialog.updateText, text);
            }
        }

        private void showCuePosition(ushort channel, uint cuePosition)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackTime[channel].CuePosition = (int)cuePosition;
            timeLine.UpdateDuration(channel, trackTime[channel].Duration - trackTime[channel].CuePosition);
        }

        private void showIntroPosition(ushort channel, uint introPosition)
        {
            if (ChannelOutOfBounds(channel)) return;
            trackTime[channel].IntroPosition = (int)introPosition;
        }
    }
}