using BAPSPresenter;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class BAPSChannel : UserControl
    {
        /// <summary>
        /// The ID of this channel.
        /// </summary>
        private ushort channelID;

        /// <summary>
        /// This channel's count-down state.
        /// </summary>
        private CountDownState cds;

        /// <summary>
        /// This channel's timeout struct.
        /// </summary>
        private ChannelTimeoutStruct cts;

        public BAPSChannel(ushort channelID)
        {
            this.channelID = channelID;

            InitializeComponent();

            cds = new CountDownState(channelID);
            length.Tag = cds; // Needed?

            cts = new ChannelTimeoutStruct(channelID, 10);
            loadImpossibleTimer.Tag = cts; // Needed?

            nearEndTimer.Tag = channelID; // Needed?
        }

        public event RequestChangeEventHandler TrackListRequestChange;
        public event EventHandler PlayRequested;
        public event EventHandler PauseRequested;
        public event EventHandler StopRequested;
        public event EventHandler<int> PositionChanged;
        public event EventHandler<int> CuePositionChanged;
        public event EventHandler<int> IntroPositionChanged;
        public event EventHandler<int> TimeLineUpdateNeeded;
        public event EventHandler<uint> TrackBarMoved;

        public event ToolStripItemClickedEventHandler TrackListContextMenuStripItemClicked;

        public void ShowPlay()
        {
            playButton.BackColor = System.Drawing.Color.DarkGreen;
            playButton.Enabled = false;
            pauseButton.BackColor = System.Drawing.SystemColors.Control;
            stopButton.BackColor = System.Drawing.SystemColors.Control;
        }

        public void ShowPause()
        {
            playButton.BackColor = System.Drawing.SystemColors.Control;
            playButton.Enabled = true;
            pauseButton.BackColor = System.Drawing.Color.DarkOrange;
            pauseButton.BackColor = System.Drawing.SystemColors.Control;
        }

        public void ShowStop()
        {
            playButton.BackColor = System.Drawing.SystemColors.Control;
            playButton.Enabled = true;
            pauseButton.BackColor = System.Drawing.SystemColors.Control;
            stopButton.BackColor = System.Drawing.Color.Firebrick;
        }

        /** Enable or disable the timer controls **/
        public void EnableTimerControls(bool shouldEnable)
        {
            length.Visible = shouldEnable;
            length.Enabled = shouldEnable;
            var cds = (CountDownState)length.Tag;
            cds.running = false;
        }

        #region Mouse events

        private void playButton_Click(object sender, EventArgs e) => PlayRequested?.Invoke(sender, e);
        private void pauseButton_Click(object sender, EventArgs e) => PauseRequested?.Invoke(sender, e);
        private void stopButton_Click(object sender, EventArgs e) => StopRequested?.Invoke(sender, e);

        private void Length_MouseDown(object sender, MouseEventArgs e)
        {
            var label = (BAPSFormControls.BAPSLabel)sender;

            if (e.Y > 15)
            {
                cds.running = !cds.running;
            }
            else if (e.X > 3 * (label.ClientRectangle.Width / 4))
            {
                if (e.Button == MouseButtons.Left)
                {
                    cds.theTime++;
                    cds.theTime = cds.theTime % 3600;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    cds.theTime--;
                    if (cds.theTime < 0)
                    {
                        cds.theTime += 3600;
                    }
                }
            }
            else if (e.X > label.ClientRectangle.Width / 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    cds.theTime += 60;
                    cds.theTime = cds.theTime % 3600;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    cds.theTime -= 60;
                    if (cds.theTime < 0)
                    {
                        cds.theTime += 3600;
                    }
                }
            }
            else
            {
                if (cds.running)
                {
                    if (cds.startAt)
                    {
                        cds.theTime += (trackTime.Duration - trackTime.CuePosition) / 1000;
                    }
                    else
                    {
                        cds.theTime += 3600;
                        cds.theTime -= (trackTime.Duration - trackTime.CuePosition) / 1000;
                    }
                    cds.theTime = cds.theTime % 3600;
                }
                cds.startAt = !cds.startAt;
            }
            if (cds.startAt)
            {
                label.InfoText = "Start At: ";
            }
            else
            {
                label.InfoText = "End At: ";
            }
            label.InfoText = string.Concat(label.InfoText, (cds.theTime / 60).ToString("00"), ":", (cds.theTime % 60).ToString("00"));
        }

        private void TrackBar_Scroll(object sender, System.EventArgs e)
        {
            var trackBar = (TrackBar)sender;
            TrackBarMoved?.Invoke(sender, (uint)trackBar.Value * 100);
        }

        #endregion Mouse events

        #region Position movement events

        private void OnPositionChanged(object sender, EventArgs e)
        {
            PositionChanged?.Invoke(sender, ((TrackTime)sender).Position);
        }

        private void OnCuePositionChanged(object sender, EventArgs e)
        {
            CuePositionChanged?.Invoke(sender, ((TrackTime)sender).CuePosition);
        }

        private void OnIntroPositionChanged(object sender, EventArgs e)
        {
            IntroPositionChanged?.Invoke(sender, ((TrackTime)sender).IntroPosition);
        }

        #endregion Position movement events

        #region Track list events

        private void TrackList_RequestChange(object o, RequestChangeEventArgs e)
        {
            Debug.Assert(e.channel == channelID);
            TrackListRequestChange?.Invoke(o, e);
        }

        private void TrackListContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var tl = (TrackList)trackListContextMenuStrip.SourceControl;
            var testValue = ConfigCache.getValueInt("Automatically advance", tl.Channel);
            var shouldCheck = testValue == ConfigCache.findChoiceIndexFor("Automatically advance", "Yes");
            automaticAdvanceToolStripMenuItem.Checked = shouldCheck;
            testValue = ConfigCache.getValueInt("Play on load", tl.Channel);
            shouldCheck = testValue == ConfigCache.findChoiceIndexFor("Play on load", "Yes");
            playOnLoadToolStripMenuItem.Checked = shouldCheck;
            testValue = ConfigCache.getValueInt("Repeat", tl.Channel);
            shouldCheck = testValue == ConfigCache.findChoiceIndexFor("Repeat", "No repeat");
            repeatNoneToolStripMenuItem.Checked = shouldCheck;
            shouldCheck = testValue == ConfigCache.findChoiceIndexFor("Repeat", "Repeat one");
            repeatOneToolStripMenuItem.Checked = shouldCheck;
            shouldCheck = testValue == ConfigCache.findChoiceIndexFor("Repeat", "Repeat all");
            repeatAllToolStripMenuItem.Checked = shouldCheck;
            deleteItemToolStripMenuItem.Enabled = tl.LastIndexClicked != -1;
        }

        private void TrackListContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TrackListContextMenuStripItemClicked?.Invoke(sender, e);
        }

        #endregion Track list events

        #region Timer events

        private void LoadImpossibleFlicker(object sender, EventArgs e)
        {
            var timer = (Timer)sender;
            var cts = (ChannelTimeoutStruct)timer.Tag;
            cts.timeout--;
            if (cts.timeout == 0)
            {
                timer.Enabled = false;
                loadedText.BackColor = System.Drawing.SystemColors.Window;
            }
            else
            {
                if (loadedText.BackColor == System.Drawing.SystemColors.Window)
                    loadedText.BackColor = System.Drawing.Color.LightSteelBlue;
                else
                    loadedText.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        private void NearEndFlash(object sender, EventArgs e)
        {
            var timer = (Timer)sender;
            var channel = (int)timer.Tag;
            timeLeft.Highlighted = !timeLeft.Highlighted;
        }

        public void CountdownTick()
        {
            if (playButton.Enabled && cds.running)
            {
                var dt = DateTime.Now;
                if (!cds.startAt)
                {
                    dt = dt.AddMilliseconds(trackTime.Duration - trackTime.CuePosition);
                }
                int millisecsPastHour = (((dt.Minute * 60) + dt.Second) * 1000) + dt.Millisecond;
                int value = cds.theTime * 1000;
                if (value < millisecsPastHour)
                {
                    value += 3600000;
                }
                value -= millisecsPastHour;
                int valuesecs = value / 1000;
                /** WORK NEEDED: This allows 5 seconds grace in case of heavy system load
                    *               It would be better if there were guaranteed start if it didnt kick in.
                    **/
                if (valuesecs > 3595)
                {
                    cds.running = false;
                    PlayRequested?.Invoke(this, EventArgs.Empty);
                }
                length.Text = string.Concat((valuesecs / 60).ToString("00"), ":", (valuesecs % 60).ToString("00"));

                TimeLineUpdateNeeded?.Invoke(this, value);
            }
            else
            {
                cds.running = false;
                TimeLineUpdateNeeded?.Invoke(this, -1);
                length.Text = "--:--";
            }
            if (cds.startAt)
            {
                length.InfoText = "Start At: ";
            }
            else
            {
                length.InfoText = "End At: ";
            }
            length.InfoText = string.Concat(length.InfoText, (cds.theTime / 60).ToString("00"), ":", (cds.theTime % 60).ToString("00"));
        }

        #endregion Timer events
    }
}
