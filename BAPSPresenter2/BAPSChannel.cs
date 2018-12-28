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
        /// <remarks>
        /// The first time this property is accessed, we retrieve the
        /// channel ID from this channel's Tag.  The tag must be
        /// a string.
        /// </remarks>
        internal ushort ChannelID
        {
            get
            {
                if (_channelID == null)
                {
                    var t = Tag as string;
                    Debug.Assert(t != null, "Tried to get channel ID before it is set");
                    _channelID = ushort.Parse(t);
                    SetupTimers();
                }
                return _channelID ?? 0;
            }
        }
        private ushort? _channelID = null;

        /// <summary>
        /// This channel's count-down state.
        /// </summary>
        private CountDownState cds = null;

        /// <summary>
        /// This channel's timeout struct.
        /// </summary>
        private ChannelTimeoutStruct cts = null;

        public int LoadedTextIndex { set { trackList.LoadedTextIndex = value; } }

        public BAPSChannel() : base()
        {
            InitializeComponent();
        }

        private void SetupTimers()
        {
            Debug.Assert(_channelID != null, "should have set channel ID before doing this");

            cds = new CountDownState(ChannelID);
            length.Tag = cds; // Needed?

            cts = new ChannelTimeoutStruct(ChannelID, 10);
            loadImpossibleTimer.Tag = cts; // Needed?

            nearEndTimer.Tag = _channelID; // Needed?
        }

        #region Events used to talk to the main presenter

        public event RequestChangeEventHandler TrackListRequestChange;
        public event EventHandler<ChannelOperationLookup> OpRequest;
        public event PositionRequestChangeEventHandler PositionRequestChange;
        public event TimelineChangeEventHandler TimelineChanged;
        public event EventHandler<uint> TrackBarMoved;
        public event ToolStripItemClickedEventHandler TrackListContextMenuStripItemClicked;

        #endregion Events used to talk to the main presenter

        private void RequestTimelineChange(TimelineChangeType type, int value)
        {
            TimelineChanged?.Invoke(this, new TimelineChangeEventArgs(ChannelID, type, value));
        }

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

            var hours = Math.DivRem(secs, 3600, out _);
            int mins = Math.DivRem(secs, 60, out _) - (hours * 60);

            secs = secs - ((mins * 60) + (hours * 3600));

            return TimeToString(hours, mins, secs, msecs % 1000 / 10);
        }

        #region Updating the channel's view

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
            stopButton.BackColor = System.Drawing.SystemColors.Control;
        }

        public void ShowStop()
        {
            playButton.BackColor = System.Drawing.SystemColors.Control;
            playButton.Enabled = true;
            pauseButton.BackColor = System.Drawing.SystemColors.Control;
            stopButton.BackColor = System.Drawing.Color.Firebrick;
        }

        public int DisplayedPosition
        {
            set
            {
                /** Channels are ready when they have a valid duration **/
                if (trackTime.Duration >= value)
                {
                    trackTime.Position = value;
                    RequestTimelineChange(TimelineChangeType.Position, value - trackTime.CuePosition);

                    value = (int)(Math.Round(value / 1000f) * 1000);
                    /** Set the amount of time gone **/
                    timeGone.Text = MillisecondsToTimeString(value);
                    var timeleft = trackTime.Duration - value;
                    timeLeft.Text = MillisecondsToTimeString(timeleft);
                    if (playButton.Enabled || timeleft > 10000 || timeleft < 500)
                    {
                        nearEndTimer.Enabled = false;
                        timeLeft.Highlighted = false;
                    }
                    else if (!playButton.Enabled)
                    {
                        nearEndTimer.Interval = 100;
                        nearEndTimer.Enabled = true;
                    }
                }
                else
                {
                    /** WORK NEEDED: there is a problem **/
                }
            }
        }

        public int DisplayedCuePosition
        {
            set
            {
                trackTime.CuePosition = value;
                RequestTimelineChange(TimelineChangeType.Duration, trackTime.Duration - value);
            }
        }

        public int DisplayedDuration
        {
            set
            {
                trackTime.Position = 0;
                trackTime.Duration = value;
                RequestTimelineChange(TimelineChangeType.Duration, value - trackTime.CuePosition);
            }
        }

        public int DisplayedIntroPosition
        {
            set
            {
                trackTime.IntroPosition = value;
            }
        }

        internal void ShowLoadedItem(uint index, Command itemType, string description)
        {
            trackList.LoadedIndex = (int)index;
            loadedText.Text = description;
            cds.running = false;
            if (itemType == Command.VOIDITEM)
            {
                trackTime.Position = 0;
                RequestTimelineChange(TimelineChangeType.Position, 0);
                trackTime.Duration = 0;
                RequestTimelineChange(TimelineChangeType.Duration, 0);
                trackTime.CuePosition = 0;
                trackTime.IntroPosition = 0;
                timeLeft.Text = MillisecondsToTimeString(0);
                timeGone.Text = MillisecondsToTimeString(0);
                nearEndTimer.Enabled = false;
                timeLeft.Highlighted = false;
            }
        }

        internal void AddTrack(uint index, uint type, string description)
        {
            /** Add an item to the end of the list ( only method currently supported by server ) **/
            trackList.addTrack((int)type, description);
        }

        internal void MoveTrack(uint oldIndex, uint newIndex)
        {
            trackList.moveTrack((int)oldIndex, (int)newIndex);
        }

        internal void RemoveTrack(uint index) => trackList.removeTrack((int)index);

        internal void CleanPlaylist()
        {
            trackList.clearTrackList();
            cds.running = false;
        }

        #endregion Updating the channel view

        /** Enable or disable the timer controls **/
        public void EnableTimerControls(bool shouldEnable)
        {
            Debug.Assert(_channelID != null, "shouldn't enable timer controls before setting channel ID"); 
            length.Visible = shouldEnable;
            length.Enabled = shouldEnable;
            cds.running = false;
        }

        #region Mouse events

        private void RequestOp(Command command)
        {
            // TODO(@MattWindsor91): decouple this from BAPSNET commands.
            OpRequest?.Invoke(this, new ChannelOperationLookup(ChannelID, (ushort)command));
        }

        private void playButton_Click(object sender, EventArgs e) => RequestOp(Command.PLAY);
        private void pauseButton_Click(object sender, EventArgs e)
        {
            trackList.clearPendingLoadRequest();
            RequestOp(Command.PAUSE);
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            trackList.clearPendingLoadRequest();
            RequestOp(Command.STOP);
        }

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

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            var trackBar = (TrackBar)sender;
            TrackBarMoved?.Invoke(sender, (uint)trackBar.Value * 100);
        }

        #endregion Mouse events

        #region Position movement events

        private void OnPositionRequestChange(PositionType type, int requestedValue)
        {
            PositionRequestChange?.Invoke(this, new PositionRequestChange(ChannelID, type, requestedValue));
        }


        private void OnPositionChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == trackTime, "Got position change request from unexpected place");
            OnPositionRequestChange(PositionType.Position, trackTime.Position);
        }

        private void OnCuePositionChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == trackTime, "Got position change request from unexpected place");
            OnPositionRequestChange(PositionType.Cue, trackTime.CuePosition);
        }

        private void OnIntroPositionChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == trackTime, "Got position change request from unexpected place");
            OnPositionRequestChange(PositionType.Intro, trackTime.IntroPosition);

        }

        #endregion Position movement events

        #region Track list events

        private bool IsTextItemAt(int index) =>
            (Command)trackList.items[index].type == Command.TEXTITEM;

        private bool IsLoadPossible(int index) =>
            IsTextItemAt(index) || playButton.Enabled;

        private void TrackList_RequestChange(object o, RequestChangeEventArgs e)
        {
            Debug.Assert(e.channel == ChannelID);

            // Don't propagate impossible loads outside the channel.
            if ((ChangeType)e.ct == ChangeType.SELECTEDINDEX)
            {
                if (!IsLoadPossible(e.index))
                {
                    cts.timeout = 10;
                    loadImpossibleTimer.Enabled = true;
                    return;
                }
                loadImpossibleTimer.Enabled = false;
                loadedText.BackColor = System.Drawing.SystemColors.Window;
            }

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
            var tl = (TrackList)trackListContextMenuStrip.SourceControl;
            TrackListContextMenuStripItemClicked?.Invoke(tl /* TODO(@MattWindsor91): this is a hack! */, e);
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
                    RequestOp(Command.PLAY);
                }
                length.Text = string.Concat((valuesecs / 60).ToString("00"), ":", (valuesecs % 60).ToString("00"));

                RequestTimelineChange(TimelineChangeType.Start, value);
            }
            else
            {
                cds.running = false;
                RequestTimelineChange(TimelineChangeType.Start, -1);
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

        internal void UpdateCountDown(int theTime)
        {
            cds.startAt = true;
            cds.theTime = theTime;
            cds.running = true;
        }

        #endregion Timer events
    }
}
