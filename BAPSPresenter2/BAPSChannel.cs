using BAPSClientCommon;
using BAPSPresenter;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;

namespace BAPSPresenter2
{
    public partial class BAPSChannel : UserControl
    {
        /// <summary>
        /// The ID of this channel.
        /// <para>
        /// This may be set only once (and should be set in the designer, if applicable).
        /// Once set to a valid (non-negative) value for the first time, the
        /// various parts of the channel that depend on the channel IDs will be
        /// initialised.
        /// </para>
        /// </summary>
        public int ChannelId
        {
            get => _channelId;
            set
            {
                if (0 <= _channelId) throw new InvalidOperationException("Can't set a channel ID multiple times");
                _channelId = value;
                trackList.Channel = (ushort)value;
                trackTime.Channel = value;
                if (0 <= _channelId) SetupTimers();
            }
        }
        private int _channelId = -1;

        /// <summary>
        /// This channel's count-down state.
        /// </summary>
        private CountDownState _cds;

        /// <summary>
        /// This channel's timeout struct.
        /// </summary>
        private ChannelTimeoutStruct _cts;

        public int LoadedTextIndex { set => trackList.LoadedTextIndex = value; }

        public BAPSChannel()
        {
            InitializeComponent();
        }

        private void SetupTimers()
        {
            _cds = new CountDownState(ChannelId);
            length.Tag = _cds; // Needed?

            _cts = new ChannelTimeoutStruct(ChannelId, 10);
            loadImpossibleTimer.Tag = _cts; // Needed?

            nearEndTimer.Tag = _channelId; // Needed?
        }

        #region Events used to talk to the main presenter

        public event RequestChangeEventHandler TrackListRequestChange;

        /// <summary>
        ///     Event that fires when the channel wants to change its state.
        ///     <para>
        ///         The use of the 'Updates' event here is slightly
        ///         misleading: this is a server request, not a response.
        ///         We re-use the same event because we're sending the exact
        ///         same data.
        ///     </para>
        /// </summary>
        public event Updates.ChannelStateEventHandler OpRequest;
        public event Requests.ChannelMarkerEventHandler PositionRequestChange;
        public event TimelineChangeEventHandler TimelineChanged;
        public event EventHandler<uint> TrackBarMoved;
        public event ChannelConfigChangeHandler ChannelConfigChange;
        public event Updates.ItemDeleteEventHandler ItemDeleteRequest;
        public event Updates.ChannelResetEventHandler ChannelResetRequest;
        public event EventHandler<TrackList> AudioWallRequest;

        #endregion Events used to talk to the main presenter

        private void RequestTimelineChange(TimelineChangeType type, int value)
        {
            var id = ChannelId;
            if (id < 0) return;
            TimelineChanged?.Invoke(this, new TimelineChangeEventArgs((ushort)id, type, value));
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
                    timeGone.Text = BAPSClientCommon.Utils.Time.MillisecondsToTimeString(value);
                    var tl = trackTime.Duration - value;
                    timeLeft.Text = BAPSClientCommon.Utils.Time.MillisecondsToTimeString(tl);
                    if (playButton.Enabled || tl > 10000 || tl < 500)
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
            set => trackTime.IntroPosition = value;
        }

        internal void ShowLoadedItem(uint index, Track entry)
        {
            trackList.LoadedIndex = (int)index;
            loadedText.Text = entry.Description;
            _cds.running = false;
            if (!entry.IsAudioItem && !entry.IsTextItem)
            {
                trackTime.Position = 0;
                RequestTimelineChange(TimelineChangeType.Position, 0);
                trackTime.Duration = 0;
                RequestTimelineChange(TimelineChangeType.Duration, 0);
                trackTime.CuePosition = 0;
                trackTime.IntroPosition = 0;
                timeLeft.Text = BAPSClientCommon.Utils.Time.MillisecondsToTimeString(0);
                timeGone.Text = BAPSClientCommon.Utils.Time.MillisecondsToTimeString(0);
                nearEndTimer.Enabled = false;
                timeLeft.Highlighted = false;
            }
        }

        internal void AddTrack(uint index, Track entry)
        {
            /** Add an item to the end of the list ( only method currently supported by server ) **/
            trackList.AddTrack(entry);
        }

        internal void MoveTrack(uint oldIndex, uint newIndex)
        {
            trackList.MoveTrack((int)oldIndex, (int)newIndex);
        }

        internal void RemoveTrack(uint index) => trackList.RemoveTrack((int)index);

        internal void CleanPlaylist()
        {
            trackList.ClearTrackList();
            _cds.running = false;
        }

        #endregion Updating the channel view

        /** Enable or disable the timer controls **/
        public void EnableTimerControls(bool shouldEnable)
        {
            length.Visible = shouldEnable;
            length.Enabled = shouldEnable;
            _cds.running = false;
        }

        #region Mouse events

        private void RequestStateChange(ChannelState state)
        {
            OpRequest?.Invoke(this, new Updates.ChannelStateEventArgs((ushort)ChannelId, state));
        }

        private void playButton_Click(object sender, EventArgs e) => RequestStateChange(ChannelState.Playing);
        private void pauseButton_Click(object sender, EventArgs e)
        {
            trackList.ClearPendingLoadRequest();
            RequestStateChange(ChannelState.Paused);
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            trackList.ClearPendingLoadRequest();
            RequestStateChange(ChannelState.Stopped);
        }

        private void Length_MouseDown(object sender, MouseEventArgs e)
        {
            var label = (BAPSFormControls.BAPSLabel)sender;

            if (e.Y > 15)
            {
                _cds.running = !_cds.running;
            }
            else if (e.X > 3 * (label.ClientRectangle.Width / 4))
            {
                if (e.Button == MouseButtons.Left)
                {
                    _cds.theTime++;
                    _cds.theTime = _cds.theTime % 3600;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    _cds.theTime--;
                    if (_cds.theTime < 0)
                    {
                        _cds.theTime += 3600;
                    }
                }
            }
            else if (e.X > label.ClientRectangle.Width / 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _cds.theTime += 60;
                    _cds.theTime = _cds.theTime % 3600;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    _cds.theTime -= 60;
                    if (_cds.theTime < 0)
                    {
                        _cds.theTime += 3600;
                    }
                }
            }
            else
            {
                if (_cds.running)
                {
                    if (_cds.startAt)
                    {
                        _cds.theTime += (trackTime.Duration - trackTime.CuePosition) / 1000;
                    }
                    else
                    {
                        _cds.theTime += 3600;
                        _cds.theTime -= (trackTime.Duration - trackTime.CuePosition) / 1000;
                    }
                    _cds.theTime = _cds.theTime % 3600;
                }
                _cds.startAt = !_cds.startAt;
            }
            label.InfoText = _cds.startAt ? "Start At: " : "End At: ";
            label.InfoText = string.Concat(label.InfoText, (_cds.theTime / 60).ToString("00"), ":", (_cds.theTime % 60).ToString("00"));
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            var trackBar = (TrackBar)sender;
            TrackBarMoved?.Invoke(sender, (uint)trackBar.Value * 100);
        }

        #endregion Mouse events

        #region Position movement events

        private void OnPositionChanged(object sender, Requests.ChannelMarkerEventArgs e)
        {
            Debug.Assert(sender == trackTime, "Got position change request from unexpected place");
            Debug.Assert(e.ChannelId == ChannelId, "Got position change request for unexpected channel");
            PositionRequestChange?.Invoke(this, e);
        }

        #endregion Position movement events

        #region Track list events

        private bool IsLoadPossible(int index) =>
            trackList.IsTextItemAt(index) || playButton.Enabled;

        private void TrackList_RequestChange(object o, RequestChangeEventArgs e)
        {
            // Don't propagate impossible loads outside the channel.
            if (e.ct == ChangeType.SELECTEDINDEX)
            {
                if (!IsLoadPossible(e.index))
                {
                    _cts.timeout = 10;
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
            var config = Main.Config;
            var testValue = config.GetChoice("Auto Advance", ChannelId);
            automaticAdvanceToolStripMenuItem.Checked = testValue == "Yes";
            testValue = config.GetChoice("Play on load", ChannelId);
            playOnLoadToolStripMenuItem.Checked = testValue == "Yes";
            testValue = config.GetChoice("Repeat", ChannelId);
            repeatNoneToolStripMenuItem.Checked = testValue == "No repeat";
            repeatOneToolStripMenuItem.Checked = testValue == "Repeat one";
            repeatAllToolStripMenuItem.Checked = testValue == "Repeat all";
            deleteItemToolStripMenuItem.Enabled = trackList.LastIndexClicked != -1;
        }

        private void OnChannelConfigChange(ChannelConfigChangeType type) =>
            ChannelConfigChange?.Invoke(this, new ChannelConfigChangeArgs((ushort)ChannelId, type));

        private void TrackListContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == automaticAdvanceToolStripMenuItem)
            {
                var rq = ChannelConfigChangeType.AutoAdvance;
                rq |= automaticAdvanceToolStripMenuItem.Checked ? ChannelConfigChangeType.Off : ChannelConfigChangeType.On;
                OnChannelConfigChange(rq);
            }
            else if (e.ClickedItem == playOnLoadToolStripMenuItem)
            {
                var rq = ChannelConfigChangeType.PlayOnLoad;
                rq |= playOnLoadToolStripMenuItem.Checked ? ChannelConfigChangeType.Off : ChannelConfigChangeType.On;
                OnChannelConfigChange(rq);
            }
            else if (e.ClickedItem == repeatNoneToolStripMenuItem && !repeatNoneToolStripMenuItem.Checked)
            {
                OnChannelConfigChange(ChannelConfigChangeType.RepeatNone);
            }
            else if (e.ClickedItem == repeatOneToolStripMenuItem && !repeatOneToolStripMenuItem.Checked)
            {
                OnChannelConfigChange(ChannelConfigChangeType.RepeatOne);
            }
            else if (e.ClickedItem == repeatAllToolStripMenuItem && !repeatAllToolStripMenuItem.Checked)
            {
                OnChannelConfigChange(ChannelConfigChangeType.RepeatAll);
            }
            else if (e.ClickedItem == deleteItemToolStripMenuItem)
            {
                var indexToDelete = (uint) trackList.LastIndexClicked;
                OnItemDeleteRequest(new Updates.ItemDeleteEventArgs((ushort)ChannelId, indexToDelete));
            }
            else if (e.ClickedItem == resetChannelStripMenuItem)
            {
                OnChannelResetRequest(new Updates.ChannelResetEventArgs((ushort)ChannelId));
            }
            else if (e.ClickedItem == showAudioWallToolStripMenuItem)
            {
                OnAudioWallRequest(trackList);
            }
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
                loadedText.BackColor = loadedText.BackColor == System.Drawing.SystemColors.Window ? System.Drawing.Color.LightSteelBlue : System.Drawing.SystemColors.Window;
            }
        }

        private void NearEndFlash(object sender, EventArgs e)
        {
            timeLeft.Highlighted = !timeLeft.Highlighted;
        }

        public void CountdownTick()
        {
            if (playButton.Enabled && _cds.running)
            {
                UpdateRunningCountdown();
            }
            else
            {
                _cds.running = false;
                RequestTimelineChange(TimelineChangeType.Start, -1);
                length.Text = "--:--";
            }
        }

        private void UpdateRunningCountdown()
        {
            var dt = DateTime.Now;
            if (!_cds.startAt)
            {
                dt = dt.AddMilliseconds(trackTime.Duration - trackTime.CuePosition);
            }
            int millisecsPastHour = (((dt.Minute * 60) + dt.Second) * 1000) + dt.Millisecond;
            int value = _cds.theTime * 1000;
            if (value < millisecsPastHour)
            {
                value += 3600000;
            }
            value -= millisecsPastHour;
            int valuesecs = value / 1000;
            /** WORK NEEDED: This allows 5 seconds grace in case of heavy system load
                *               It would be better if there were guaranteed start if it didn't kick in.
                **/
            if (valuesecs > 3595)
            {
                _cds.running = false;
                RequestStateChange(ChannelState.Playing);
            }
            length.Text = string.Concat((valuesecs / 60).ToString("00"), ":", (valuesecs % 60).ToString("00"));

            RequestTimelineChange(TimelineChangeType.Start, value);
        }

        internal void UpdateCountDown(int theTime)
        {
            _cds.startAt = true;
            _cds.theTime = theTime;
            _cds.running = true;
        }

        #endregion Timer events

        protected virtual void OnItemDeleteRequest(Updates.ItemDeleteEventArgs e)
        {
            ItemDeleteRequest?.Invoke(this, e);
        }

        protected virtual void OnAudioWallRequest(TrackList e)
        {
            AudioWallRequest?.Invoke(this, e);
        }

        protected virtual void OnChannelResetRequest(Updates.ChannelResetEventArgs e)
        {
            ChannelResetRequest?.Invoke(this, e);
        }
    }
}
