using System;
using System.Windows.Forms;
using BAPSPresenter;

namespace BAPSPresenter2
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (msgQueue != null)
            {
                /** When we die it is only fair to tell the server **/
                var cmd = Command.SYSTEM | Command.END;
                msgQueue.Enqueue(new ActionMessageString((ushort)cmd, "Normal Termination"));
                /** Wait 500ms for the command to be sent **/
                int timeout = 500;
                while (msgQueue.Count > 0 && timeout > 0)
                {
                    System.Threading.Thread.Sleep(1);
                    timeout--;
                }
            }
            /** Notify the send/receive threads they should die **/
            dead = true;
            /** Empty the config cache **/
            ConfigCache.closeConfigCache();
            /** Force the receive thread to abort FIRST so that we cant receive
                any messages that need automatic responses **/
            if (receiverThread != null)
            {
                receiverThread.Abort();
                receiverThread.Join();
            }
            /** Force the sender thread to die (should be dead already) **/
            if (senderThread != null)
            {
                senderThread.Abort();
                senderThread.Join();
            }
            /** Close the connection properly **/
            clientSocket?.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /** Sub-form handles **/
        RecordLibrarySearch recordLibrarySearch = null;
        ConfigDialog configDialog = null;
        LoadShowDialog loadShowDialog = null;
        SecurityDialog securityDialog = null;
        AboutDialog about = null;
        TextDialog textDialog = null;
        AudioWall audioWall = null;

        /** Arrays with channel number indices for easy updating **/
        private ListBox[] directoryList;
        private Button[] directoryRefresh;
        private Label[] loadedText;
        private BAPSFormControls.BAPSLabel[] trackLengthText;
        private BAPSFormControls.BAPSLabel[] timeLeftText;
        private BAPSFormControls.BAPSLabel[] timeGoneText;
        private Button[] channelPlay;
        private Button[] channelPause;
        private Button[] channelStop;
        private TrackTime[] trackTime;
        private Timer[] loadImpossibleTimer;
        private Timer[] nearEndTimer;
        private Timer countdownTimer;
        private ContextMenuStrip trackListContextMenuStrip;

        private ToolStripMenuItem resetChannelStripMenuItem;

        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem deleteItemToolStripMenuItem;
        private ToolStripMenuItem automaticAdvanceToolStripMenuItem;
        private ToolStripMenuItem playOnLoadToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem repeatAllToolStripMenuItem;
        private ToolStripMenuItem repeatNoneToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;

        private ToolStripMenuItem repeatOneToolStripMenuItem;
        private TextBox MainTextDisplay;

        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem showAudioWallToolStripMenuItem;
        private TimeLine timeLine;
        private ListBox Directory0;
        private ListBox Directory1;
        private ListBox Directory2;
        private Button loadShowButton;
        private Button bapsButton1;
        private Button Directory0Refresh;
        private Button Directory1Refresh;
        private Button Directory2Refresh;
        private Button Channel0Play;
        private Button Channel0Pause;
        private Button Channel0Stop;
        private Button Channel1Play;
        private Button Channel1Pause;
        private Button Channel1Stop;
        private Button Channel2Play;
        private Button Channel2Pause;
        private Button Channel2Stop;
        private Label Channel0LoadedText;
        private Label Channel1LoadedText;
        private Label Channel2LoadedText;

        TrackList[] trackList;

        /** functions to receive events from the custom TrackTime class */
        private void positionChanged(object sender, System.EventArgs e)
        {
            var tt = (TrackTime)sender;
            var cmd = Command.PLAYBACK | Command.POSITION | (Command)tt.Channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)tt.Position));
        }

        private void cuePositionChanged(object sender, System.EventArgs e)
        {
            var tt = (TrackTime)sender;
            var cmd = Command.PLAYBACK | Command.CUEPOSITION | (Command)tt.Channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)tt.CuePosition));
        }

        private void introPositionChanged(object sender, System.EventArgs e)
        {
            var tt = (TrackTime)sender;
            var cmd = Command.PLAYBACK | Command.INTROPOSITION | (Command)tt.Channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)tt.IntroPosition));
        }

        /** functions to receive context menu events **/
        private void ChannelListClear_Click(object sender, System.EventArgs e)
        {
            var mi = (MenuItem)sender;
            var cm = (ContextMenu)mi.Parent;
            var tl = (TrackList)cm.SourceControl;
            var cmd = Command.PLAYLIST | Command.RESETPLAYLIST | (Command)tl.Channel;
            msgQueue.Enqueue(new ActionMessage((ushort)cmd));
        }

        /** ### DESIGNER PRIVATE EVENT HANDLERS ### **/
        public void BAPSPresenterMain_KeyDown(object sender, KeyEventArgs e) {
            /** Placeholder for the command we generate **/
            Command cmd;
            switch (e.KeyCode)
            {
                /** Keys F1-F4 are for channel 0 **/
                case Keys.F1: /** F1 play **/
                    cmd = Command.PLAYBACK | Command.PLAY | 0;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F2: /** F2 Pause **/
                    cmd = Command.PLAYBACK | Command.PAUSE | 0;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F3: /** F3 Stop **/
                    cmd = Command.PLAYBACK | Command.STOP | 0;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F4:
                    if (e.Alt)
                    {
                        e.Handled = true;
                    }
                    break;

                /** Keys F5-F8 are for channel 1 **/
                case Keys.F5: /** F5 Play **/
                    cmd = Command.PLAYBACK | Command.PLAY | (Command)1;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F6: /** F6 Pause **/
                    cmd = Command.PLAYBACK | Command.PAUSE | (Command)1;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F7: /** F7 Stop **/
                    cmd = Command.PLAYBACK | Command.STOP | (Command)1;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;

                /** Keys F9-F12 are for channel 2 **/
                case Keys.F9: /** F9 Play **/
                    cmd = Command.PLAYBACK | Command.PLAY | (Command)2;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F10: /** F10 Pause **/
                    cmd = Command.PLAYBACK | Command.PAUSE | (Command)2;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                case Keys.F11: /** F11 Stop **/
                    cmd = Command.PLAYBACK | Command.STOP | (Command)2;
                    msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    e.Handled = true;
                    break;
                /** Other keyboard Commands **/
                case Keys.Q: /** Ctrl+Alt+Q Shutdown **/
                    if (e.Control && e.Alt)
                    {
                        System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Do you wish to disconnect?", "Shutdown?", System.Windows.Forms.MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.Close();
                        }
                        e.Handled = true;
                    }
                    break;
                case Keys.W: /** Ctrl+Alt+W Alter Window **/
                    if (e.Control && e.Alt)
                    {
                        if (this.WindowState == System.Windows.Forms.FormWindowState.Normal)
                        {
                            System.Drawing.Rectangle bounds = System.Windows.Forms.Screen.GetWorkingArea(this);
                            bounds.X = 0;
                            bounds.Y = 0;
                            this.MaximizedBounds = bounds;
                            this.MaximumSize = bounds.Size;
                            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                        }
                        else
                        {
                            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
                        }
                    }
                    break;
                case Keys.O:
                    if (e.Control)
                    {
                         // TODO(@MattWindsor91): port these
#if false
                        if (e.Shift)
                        {
                            LocalConfigDialog ccd = new LocalConfigDialog(this);
                            ccd.ShowDialog();
                            /** Enable or disable the timers depending on the config setting **/
                            bool enableTimers = (System.String.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0);
                            enableTimerControls(enableTimers);
                        }
                        else
                        {
                            configDialog = new ConfigDialog(this, msgQueue);
                            configDialog.ShowDialog();
                            delete configDialog;
                            configDialog = nullptr;
                        }
#endif
                        e.Handled = true;
                    }
                    break;
                case Keys.S:
                    if (e.Control)
                    {
                         // TODO(@MattWindsor91): port these
#if false
                        securityDialog = new SecurityDialog(this, msgQueue);
                        Command cmd = Command.CONFIG | Command.GETPERMISSIONS;
                        msgQueue.Enqueue(new ActionMessage(cmd));
                        securityDialog.ShowDialog();
                        delete securityDialog;
                        securityDialog = nullptr;
#endif
                        e.Handled = true;
                    }
                    break;
                case Keys.N:
                    if (e.Control)
                    {
                        // TODO(@MattWindsor91): port these
#if false
                        bool wasOpen = true;
                        if (!textDialog.Visible)
                        {
                            wasOpen = false;
                            textDialog.Show();
                            MethodInvokerStr  mi = new MethodInvokerStr(textDialog, &TextDialog.updateText);
                            array < System.Object > dd = new array < System.Object > (1) { MainTextDisplay.Text};
                            textDialog.Invoke(mi, dd);
                        }
                        if (e.Shift)
                        {
                            MethodInvoker  mi = new MethodInvoker(textDialog, &TextDialog.toggleMaximize);
                            textDialog.Invoke(mi);
                        }
                        else if (wasOpen)
                        {
                            textDialog.Hide();
                        }
#endif
                        e.Handled = true;
                    }
                    break;
                case Keys.A:
                    if (e.Control && e.Alt)
                    {
                         // TODO(@MattWindsor91): port these
                        about = new AboutDialog(this);
                        msgQueue.Enqueue(new ActionMessage((ushort) (Command.SYSTEM | Command.VERSION)));
                        about.ShowDialog();
                        about = null;
                        e.Handled = true;
                    }
                    break;
                default:
                    /** Ignore all other keys **/
                    break;
            }
        }

        private void TrackBar_Scroll(object sender, System.EventArgs e) {
            var trackBar = (TrackBar)sender;
            /** Update the server with the new value the user has selected **/
            Command cmd = Command.PLAYBACK | Command.POSITION;
            /** The tag contains the channel number as a managed int **/
            cmd |= ((Command) trackBar.Tag) & (Command)0x3f;
            /** The scrollbar value is the new setting **/
            uint intArg = (uint)trackBar.Value * 100;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, intArg));
        }

        private void RefreshDirectory_Click(object sender, System.EventArgs e)
        {
            var cmd = Command.SYSTEM | Command.LISTFILES;
            /** The senders tag refers to the folder that needs to be refreshed **/
            cmd |= ((Command)((Button)sender).Tag) & (Command)0x3f;
            msgQueue.Enqueue(new ActionMessage((ushort)cmd));
        }

        private void SearchRecordLib_Click(object sender, System.EventArgs e)
        {
#if false // TODO(@MattWindsor91): port this
	        /** We have a handle stored in the class so that data can be passed to the object **/
	        recordLibrarySearch = new RecordLibrarySearch(this, msgQueue);
	        recordLibrarySearch.ShowDialog();
	        /** Always get rid of it again **/
	        recordLibrarySearch.Dispose();
        	recordLibrarySearch = null;
#endif
        }

        private void loadShow_Click(object sender, System.EventArgs e)
        {
#if false // TODO(@MattWindsor91): port this
            /** Handle stored for same reason as record library **/
            loadShowDialog = new LoadShowDialog(this, msgQueue);
            loadShowDialog.ShowDialog();
            loadShowDialog = null;
#endif
        }

        private void Directory_MouseDown(object sender, MouseEventArgs e)
        {
            var lb = (ListBox)sender;
            var folder = (int)lb.Tag;
            //Retrieve the item at the specified location within the ListBox.
            var index = lb.SelectedIndex;
            if (0 < index) return;
            // Starts a drag-and-drop operation.
            var fts = new FolderTempStruct(index, folder);
            _ = lb.DoDragDrop(fts, DragDropEffects.Copy);
        }

        private void ChannelOperation_Click(object sender, System.EventArgs e)
        {
            var ctl = (Control)sender;
            var col = (ChannelOperationLookup)ctl.Tag;
            if (col.co != (ushort)Command.PLAY)
            {
                trackList[col.channel].clearPendingLoadRequest();
            }
            var cmd = Command.PLAYBACK | (Command)col.co | (Command)(col.channel & 0x3f);
            msgQueue.Enqueue(new ActionMessage((ushort)cmd));
        }

        private void TrackList_RequestChange(object o, RequestChangeEventArgs e)
        {
            switch ((ChangeType)e.ct)
            {
                case ChangeType.SELECTEDINDEX:
                    {
                        if (((Command)trackList[e.channel].items[e.index].type == Command.TEXTITEM) || (channelPlay[e.channel].Enabled))
                        {
                            var cmd = Command.PLAYBACK | Command.LOAD;
                            /** Channel number is in the sender's tag **/
                            cmd |= (Command)(e.channel & 0x3f);
                            /** Get the selected index as above **/
                            var intArg = (uint)e.index;
                            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, intArg));

                            loadImpossibleTimer[e.channel].Enabled = false;
                            loadedText[e.channel].BackColor = System.Drawing.SystemColors.Window;
                        }
                        else
                        {
                            ((ChannelTimeoutStruct) (loadImpossibleTimer[e.channel].Tag)).timeout = 10;
                            loadImpossibleTimer[e.channel].Enabled = true;
                        }
                    }
                    break;
                case ChangeType.MOVEINDEX:
                    {
                        var cmd = Command.PLAYLIST | (Command)(e.channel & 0x3f) | Command.MOVEITEMTO;
                        msgQueue.Enqueue(new ActionMessageU32intU32int((ushort)cmd, (uint)e.index, (uint)e.index2));
                    }
                    break;
                case ChangeType.DELETEINDEX:
                    {
                        var cmd = Command.PLAYLIST | (Command)(e.channel & 0x3f) | Command.DELETEITEM;
                        msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)e.index));
                    }
                    break;
                case ChangeType.ADD:
                    {
                        var cmd = Command.PLAYLIST | Command.ADDITEM | (Command)(e.channel & 0x3f);
                        msgQueue.Enqueue(new ActionMessageU32intU32intString((ushort)cmd, (uint)Command.FILEITEM, (uint)e.index, directoryList[e.index].Items[e.index2].ToString()));
                    }
                    break;
                case ChangeType.COPY:
                    {
                        var cmd = Command.PLAYLIST | Command.COPYITEM | (Command)(e.channel & 0x3f);
                        msgQueue.Enqueue(new ActionMessageU32intU32int((ushort)cmd, (uint)e.index, (uint)e.index2));
                    }
                    break;
            }
        }

        private void trackListContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var tl = (TrackList)trackListContextMenuStrip.SourceControl;
            var testValue = ConfigCache.getValueInt("Automatically advance", tl.Channel);
            var shouldCheck = (testValue == ConfigCache.findChoiceIndexFor("Automatically advance", "Yes"));
            automaticAdvanceToolStripMenuItem.Checked = shouldCheck;
            testValue = ConfigCache.getValueInt("Play on load", tl.Channel);
            shouldCheck = (testValue == ConfigCache.findChoiceIndexFor("Play on load", "Yes"));
            playOnLoadToolStripMenuItem.Checked = shouldCheck;
            testValue = ConfigCache.getValueInt("Repeat", tl.Channel);
            shouldCheck = (testValue == ConfigCache.findChoiceIndexFor("Repeat", "No repeat"));
            repeatNoneToolStripMenuItem.Checked = shouldCheck;
            shouldCheck = (testValue == ConfigCache.findChoiceIndexFor("Repeat", "Repeat one"));
            repeatOneToolStripMenuItem.Checked = shouldCheck;
            shouldCheck = (testValue == ConfigCache.findChoiceIndexFor("Repeat", "Repeat all"));
            repeatAllToolStripMenuItem.Checked = shouldCheck;
            deleteItemToolStripMenuItem.Enabled = (tl.LastIndexClicked != -1);
        }

        private void trackListContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tl = (TrackList)trackListContextMenuStrip.SourceControl;
            var cmd = Command.CONFIG | Command.SETCONFIGVALUE | Command.CONFIG_USEVALUEMASK | (Command)tl.Channel;
            if (e.ClickedItem == automaticAdvanceToolStripMenuItem)
            {
                var oci = ConfigCache.getOption("Automatically advance");
                var newSetting = "Yes";
                if (automaticAdvanceToolStripMenuItem.Checked)
                {
                    newSetting = "No";
                }
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == playOnLoadToolStripMenuItem)
            {
                var oci = ConfigCache.getOption("Play on load");
                var newSetting = "Yes";
                if (playOnLoadToolStripMenuItem.Checked)
                {
                    newSetting = "No";
                }
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == repeatNoneToolStripMenuItem && !repeatNoneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)oci.choiceList["No repeat"]));
            }
            else if (e.ClickedItem == repeatOneToolStripMenuItem && !repeatOneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)oci.choiceList["Repeat one"]));
            }
            else if (e.ClickedItem == repeatAllToolStripMenuItem && !repeatAllToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)oci.choiceList["Repeat all"]));
            }
            else if (e.ClickedItem == deleteItemToolStripMenuItem)
            {
                cmd = Command.PLAYLIST | Command.DELETEITEM | (Command)tl.Channel;
                msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)tl.LastIndexClicked));
            }
            else if (e.ClickedItem == resetChannelStripMenuItem)
            {
                cmd = Command.PLAYLIST | Command.RESETPLAYLIST | (Command)tl.Channel;
                msgQueue.Enqueue(new ActionMessage((ushort)cmd));
            }
            else if (e.ClickedItem == showAudioWallToolStripMenuItem)
            {
                if (audioWall == null || !audioWall.Visible)
                {
#if false // TODO(@MattWindsor91): port this
                    audioWall = new AudioWall(this, msgQueue, tl);
                    audioWall.Show();
#endif
                }
                else
                {
                    audioWall.setChannel(tl);
                    refreshAudioWall();
                }
            }
        }

        private void loadImpossibleFlicker(object sender, System.EventArgs e)
        {
            var timer = (Timer)sender;
            var cts = (ChannelTimeoutStruct)timer.Tag;
            cts.timeout--;
            if (cts.timeout == 0)
            {
                timer.Enabled = false;
                loadedText[cts.channel].BackColor = System.Drawing.SystemColors.Window;
            }
            else
            {
                if (loadedText[cts.channel].BackColor == System.Drawing.SystemColors.Window)
                    loadedText[cts.channel].BackColor = System.Drawing.Color.LightSteelBlue;
                else
                    loadedText[cts.channel].BackColor = System.Drawing.SystemColors.Window;
            }
        }

        private void nearEndFlash(object sender, System.EventArgs e)
        {
            var timer = (Timer)sender;
            var channel = (int)timer.Tag;
            timeLeftText[channel].Highlighted = !timeLeftText[channel].Highlighted;
        }

        private void ChannelLength_MouseDown(object sender, MouseEventArgs e)
        {
            var label = (BAPSFormControls.BAPSLabel)sender;
            var cds = (CountDownState)label.Tag;

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
                        cds.theTime += (trackTime[cds.channel].Duration - trackTime[cds.channel].CuePosition) / 1000;
                    }
                    else
                    {
                        cds.theTime += 3600;
                        cds.theTime -= (trackTime[cds.channel].Duration - trackTime[cds.channel].CuePosition) / 1000;
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

        private void countdownTick(object sender, System.EventArgs e)
        {
            if (!timersEnabled) goto End;

            for (int i = 0; i < 3; i++)
            {
                var cds = (CountDownState)trackLengthText[i].Tag;

                if (channelPlay[i].Enabled && cds.running)
                {
                    System.DateTime dt = System.DateTime.Now;
                    if (!cds.startAt)
                    {
                        dt = dt.AddMilliseconds(trackTime[i].Duration - trackTime[i].CuePosition);
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
                        var cmd = Command.PLAYBACK | Command.PLAY | (Command)i;
                        msgQueue.Enqueue(new ActionMessage((ushort)cmd));
                    }
                    trackLengthText[i].Text = System.String.Concat((valuesecs / 60).ToString("00"), ":", (valuesecs % 60).ToString("00"));

                    timeLine.UpdateStartTime(i, value);
                }
                else
                {
                    cds.running = false;
                    timeLine.UpdateStartTime(i, -1);
                    trackLengthText[i].Text = "--:--";
                }
                if (cds.startAt)
                {
                    trackLengthText[i].InfoText = "Start At: ";
                }
                else
                {
                    trackLengthText[i].InfoText = "End At: ";
                }
                trackLengthText[i].InfoText = string.Concat(trackLengthText[i].InfoText, (cds.theTime / 60).ToString("00"), ":", (cds.theTime % 60).ToString("00"));

            }

            End:
            timeLine.tick();
        }

        private void timeLine_StartTimeChanged(object sender, TimeLineEventArgs e)
        {
            if (!timersEnabled) return;
            var cds = (CountDownState)trackLengthText[e.channel].Tag;
            cds.startAt = true;
            cds.theTime = (e.startTime / 1000) % 3600;
            cds.running = true;
        }

        private BAPSFormControls.BAPSLabel Channel0TimeGone;
        private BAPSFormControls.BAPSLabel Channel0TimeLeft;
        private BAPSFormControls.BAPSLabel Channel0Length;
        private BAPSFormControls.BAPSLabel Channel1TimeGone;
        private BAPSFormControls.BAPSLabel Channel1TimeLeft;
        private BAPSFormControls.BAPSLabel Channel1Length;
        private BAPSFormControls.BAPSLabel Channel2TimeGone;
        private BAPSFormControls.BAPSLabel Channel2TimeLeft;
        private BAPSFormControls.BAPSLabel Channel2Length;

        private TrackTime trackTime0;
        private TrackTime trackTime1;
        private TrackTime trackTime2;
        private TrackList trackList0;
        private TrackList trackList1;
        private TrackList trackList2;


#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = (new System.ComponentModel.Container());
            this.trackListContextMenuStrip = (new System.Windows.Forms.ContextMenuStrip(this.components));
            this.resetChannelStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator2 = (new System.Windows.Forms.ToolStripSeparator());
            this.deleteItemToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator3 = (new System.Windows.Forms.ToolStripSeparator());
            this.automaticAdvanceToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.playOnLoadToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator1 = (new System.Windows.Forms.ToolStripSeparator());
            this.repeatAllToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.repeatOneToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.repeatNoneToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator4 = (new System.Windows.Forms.ToolStripSeparator());
            this.showAudioWallToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.MainTextDisplay = (new System.Windows.Forms.TextBox());
            this.Directory0 = (new System.Windows.Forms.ListBox());
            this.Directory1 = (new System.Windows.Forms.ListBox());
            this.Directory2 = (new System.Windows.Forms.ListBox());
            this.loadShowButton = (new System.Windows.Forms.Button());
            this.bapsButton1 = (new System.Windows.Forms.Button());
            this.Directory0Refresh = (new System.Windows.Forms.Button());
            this.Directory1Refresh = (new System.Windows.Forms.Button());
            this.Directory2Refresh = (new System.Windows.Forms.Button());
            this.Channel0Play = (new System.Windows.Forms.Button());
            this.Channel0Pause = (new System.Windows.Forms.Button());
            this.Channel0Stop = (new System.Windows.Forms.Button());
            this.Channel1Play = (new System.Windows.Forms.Button());
            this.Channel1Pause = (new System.Windows.Forms.Button());
            this.Channel1Stop = (new System.Windows.Forms.Button());
            this.Channel2Play = (new System.Windows.Forms.Button());
            this.Channel2Pause = (new System.Windows.Forms.Button());
            this.Channel2Stop = (new System.Windows.Forms.Button());
            this.Channel0LoadedText = (new System.Windows.Forms.Label());
            this.Channel1LoadedText = (new System.Windows.Forms.Label());
            this.Channel2LoadedText = (new System.Windows.Forms.Label());
            this.timeLine = (new BAPSPresenter.TimeLine());
            this.Channel2Length = (new BAPSFormControls.BAPSLabel());
            this.Channel1TimeLeft = (new BAPSFormControls.BAPSLabel());
            this.Channel1Length = (new BAPSFormControls.BAPSLabel());
            this.trackList0 = (new BAPSPresenter.TrackList());
            this.Channel0Length = (new BAPSFormControls.BAPSLabel());
            this.Channel2TimeLeft = (new BAPSFormControls.BAPSLabel());
            this.Channel0TimeLeft = (new BAPSFormControls.BAPSLabel());
            this.Channel1TimeGone = (new BAPSFormControls.BAPSLabel());
            this.Channel2TimeGone = (new BAPSFormControls.BAPSLabel());
            this.Channel0TimeGone = (new BAPSFormControls.BAPSLabel());
            this.trackList2 = (new BAPSPresenter.TrackList());
            this.trackList1 = (new BAPSPresenter.TrackList());
            this.trackTime1 = (new BAPSPresenter.TrackTime());
            this.trackTime2 = (new BAPSPresenter.TrackTime());
            this.trackTime0 = (new BAPSPresenter.TrackTime());
            this.trackListContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackListContextMenuStrip
            // 
            this.trackListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[12] {
                this.resetChannelStripMenuItem,
                this.toolStripSeparator2, this.deleteItemToolStripMenuItem, this.toolStripSeparator3, this.automaticAdvanceToolStripMenuItem,
                this.playOnLoadToolStripMenuItem, this.toolStripSeparator1, this.repeatAllToolStripMenuItem, this.repeatOneToolStripMenuItem,
                this.repeatNoneToolStripMenuItem, this.toolStripSeparator4, this.showAudioWallToolStripMenuItem});
            this.trackListContextMenuStrip.Name = "trackListContextMenuStrip";
            this.trackListContextMenuStrip.ShowCheckMargin = true;
            this.trackListContextMenuStrip.ShowImageMargin = false;
            this.trackListContextMenuStrip.Size = new System.Drawing.Size(178, 204);
            this.trackListContextMenuStrip.ItemClicked += trackListContextMenuStrip_ItemClicked;
            this.trackListContextMenuStrip.Opening += trackListContextMenuStrip_Opening;
            // 
            // resetChannelStripMenuItem
            // 
            this.resetChannelStripMenuItem.Name = "resetChannelStripMenuItem";
            this.resetChannelStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetChannelStripMenuItem.Text = "&Reset Channe";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // deleteItemToolStripMenuItem
            // 
            this.deleteItemToolStripMenuItem.Name = "deleteItemToolStripMenuItem";
            this.deleteItemToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.deleteItemToolStripMenuItem.Text = "&Delete Item";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(174, 6);
            // 
            // automaticAdvanceToolStripMenuItem
            // 
            this.automaticAdvanceToolStripMenuItem.Name = "automaticAdvanceToolStripMenuItem";
            this.automaticAdvanceToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.automaticAdvanceToolStripMenuItem.Text = "&Automatic advance";
            // 
            // playOnLoadToolStripMenuItem
            // 
            this.playOnLoadToolStripMenuItem.Name = "playOnLoadToolStripMenuItem";
            this.playOnLoadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.playOnLoadToolStripMenuItem.Text = "&Play on load";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // repeatAllToolStripMenuItem
            // 
            this.repeatAllToolStripMenuItem.Name = "repeatAllToolStripMenuItem";
            this.repeatAllToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatAllToolStripMenuItem.Text = "Repeat a&l";
            // 
            // repeatOneToolStripMenuItem
            // 
            this.repeatOneToolStripMenuItem.Name = "repeatOneToolStripMenuItem";
            this.repeatOneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatOneToolStripMenuItem.Text = "Repeat &one";
            // 
            // repeatNoneToolStripMenuItem
            // 
            this.repeatNoneToolStripMenuItem.Name = "repeatNoneToolStripMenuItem";
            this.repeatNoneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.repeatNoneToolStripMenuItem.Text = "Repeat &none";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            // 
            // showAudioWallToolStripMenuItem
            // 
            this.showAudioWallToolStripMenuItem.Name = "showAudioWallToolStripMenuItem";
            this.showAudioWallToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showAudioWallToolStripMenuItem.Text = "&Show AudioWal";
            // 
            // MainTextDisplay
            // 
            this.MainTextDisplay.AcceptsReturn = true;
            this.MainTextDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainTextDisplay.Font = (new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.MainTextDisplay.Location = new System.Drawing.Point(8, 647);
            this.MainTextDisplay.Multiline = true;
            this.MainTextDisplay.Name = "MainTextDisplay";
            this.MainTextDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MainTextDisplay.Size = new System.Drawing.Size(1024, 82);
            this.MainTextDisplay.TabIndex = 29;
            this.MainTextDisplay.Text = "<You can type notes here>";
            // 
            // Directory0
            // 
            this.Directory0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory0.Location = new System.Drawing.Point(12, 116);
            this.Directory0.Name = "Directory0";
            this.Directory0.Size = new System.Drawing.Size(234, 132);
            this.Directory0.TabIndex = 215;
            this.Directory0.MouseDown += Directory_MouseDown;
            // 
            // Directory1
            // 
            this.Directory1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory1.Location = new System.Drawing.Point(12, 276);
            this.Directory1.Name = "Directory1";
            this.Directory1.Size = new System.Drawing.Size(234, 132);
            this.Directory1.TabIndex = 216;
            this.Directory1.MouseDown += Directory_MouseDown;
            // 
            // Directory2
            // 
            this.Directory2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Directory2.Location = new System.Drawing.Point(12, 436);
            this.Directory2.Name = "Directory2";
            this.Directory2.Size = new System.Drawing.Size(234, 132);
            this.Directory2.TabIndex = 217;
            this.Directory2.MouseDown += Directory_MouseDown;
            // 
            // loadShowButton
            // 
            this.loadShowButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadShowButton.Font = (new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.loadShowButton.Location = new System.Drawing.Point(12, 12);
            this.loadShowButton.Name = "loadShowButton";
            this.loadShowButton.Size = new System.Drawing.Size(114, 64);
            this.loadShowButton.TabIndex = 218;
            this.loadShowButton.Text = "Load Show";
            this.loadShowButton.Click += loadShow_Click;
            // 
            // bapsButton1
            // 
            this.bapsButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bapsButton1.Font = (new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.bapsButton1.Location = new System.Drawing.Point(132, 12);
            this.bapsButton1.Name = "bapsButton1";
            this.bapsButton1.Size = new System.Drawing.Size(114, 64);
            this.bapsButton1.TabIndex = 219;
            this.bapsButton1.Text = "Search Library";
            this.bapsButton1.Click += SearchRecordLib_Click;
            // 
            // Directory0Refresh
            // 
            this.Directory0Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory0Refresh.Location = new System.Drawing.Point(12, 94);
            this.Directory0Refresh.Name = "Directory0Refresh";
            this.Directory0Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory0Refresh.TabIndex = 220;
            this.Directory0Refresh.Click += RefreshDirectory_Click;
            // 
            // Directory1Refresh
            // 
            this.Directory1Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory1Refresh.Location = new System.Drawing.Point(12, 254);
            this.Directory1Refresh.Name = "Directory1Refresh";
            this.Directory1Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory1Refresh.TabIndex = 221;
            this.Directory1Refresh.Click += RefreshDirectory_Click;
            // 
            // Directory2Refresh
            // 
            this.Directory2Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Directory2Refresh.Location = new System.Drawing.Point(12, 414);
            this.Directory2Refresh.Name = "Directory2Refresh";
            this.Directory2Refresh.Size = new System.Drawing.Size(234, 23);
            this.Directory2Refresh.TabIndex = 222;
            this.Directory2Refresh.Click += RefreshDirectory_Click;
            // 
            // Channel0Play
            // 
            this.Channel0Play.BackColor = System.Drawing.SystemColors.Control;
            this.Channel0Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Play.Location = new System.Drawing.Point(252, 415);
            this.Channel0Play.Name = "Channel0Play";
            this.Channel0Play.Size = new System.Drawing.Size(75, 23);
            this.Channel0Play.TabIndex = 223;
            this.Channel0Play.Text = "F1 - Play";
            this.Channel0Play.UseVisualStyleBackColor = false;
            this.Channel0Play.Click += ChannelOperation_Click;
            // 
            // Channel0Pause
            // 
            this.Channel0Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Pause.Location = new System.Drawing.Point(343, 415);
            this.Channel0Pause.Name = "Channel0Pause";
            this.Channel0Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel0Pause.TabIndex = 223;
            this.Channel0Pause.Text = "F2 - Pause";
            this.Channel0Pause.UseVisualStyleBackColor = true;
            this.Channel0Pause.Click += ChannelOperation_Click;
            // 
            // Channel0Stop
            // 
            this.Channel0Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel0Stop.Location = new System.Drawing.Point(433, 415);
            this.Channel0Stop.Name = "Channel0Stop";
            this.Channel0Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel0Stop.TabIndex = 223;
            this.Channel0Stop.Text = "F3 - Stop";
            this.Channel0Stop.UseVisualStyleBackColor = true;
            this.Channel0Stop.Click += ChannelOperation_Click;
            // 
            // Channel1Play
            // 
            this.Channel1Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Play.Location = new System.Drawing.Point(514, 415);
            this.Channel1Play.Name = "Channel1Play";
            this.Channel1Play.Size = new System.Drawing.Size(75, 23);
            this.Channel1Play.TabIndex = 223;
            this.Channel1Play.Text = "F5 - Play";
            this.Channel1Play.UseVisualStyleBackColor = true;
            this.Channel1Play.Click += ChannelOperation_Click;
            // 
            // Channel1Pause
            // 
            this.Channel1Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Pause.Location = new System.Drawing.Point(604, 415);
            this.Channel1Pause.Name = "Channel1Pause";
            this.Channel1Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel1Pause.TabIndex = 223;
            this.Channel1Pause.Text = "F6 - Pause";
            this.Channel1Pause.UseVisualStyleBackColor = true;
            this.Channel1Pause.Click += ChannelOperation_Click;
            // 
            // Channel1Stop
            // 
            this.Channel1Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel1Stop.Location = new System.Drawing.Point(695, 415);
            this.Channel1Stop.Name = "Channel1Stop";
            this.Channel1Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel1Stop.TabIndex = 223;
            this.Channel1Stop.Text = "F7 - Stop";
            this.Channel1Stop.UseVisualStyleBackColor = true;
            this.Channel1Stop.Click += ChannelOperation_Click;
            // 
            // Channel2Play
            // 
            this.Channel2Play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Play.Location = new System.Drawing.Point(776, 415);
            this.Channel2Play.Name = "Channel2Play";
            this.Channel2Play.Size = new System.Drawing.Size(75, 23);
            this.Channel2Play.TabIndex = 223;
            this.Channel2Play.Text = "F9 - Play";
            this.Channel2Play.UseVisualStyleBackColor = true;
            this.Channel2Play.Click += ChannelOperation_Click;
            // 
            // Channel2Pause
            // 
            this.Channel2Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Pause.Location = new System.Drawing.Point(866, 415);
            this.Channel2Pause.Name = "Channel2Pause";
            this.Channel2Pause.Size = new System.Drawing.Size(75, 23);
            this.Channel2Pause.TabIndex = 223;
            this.Channel2Pause.Text = "F10 - Pause";
            this.Channel2Pause.UseVisualStyleBackColor = true;
            this.Channel2Pause.Click += ChannelOperation_Click;
            // 
            // Channel2Stop
            // 
            this.Channel2Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Channel2Stop.Location = new System.Drawing.Point(957, 415);
            this.Channel2Stop.Name = "Channel2Stop";
            this.Channel2Stop.Size = new System.Drawing.Size(75, 23);
            this.Channel2Stop.TabIndex = 223;
            this.Channel2Stop.Text = "F11 - Stop";
            this.Channel2Stop.UseVisualStyleBackColor = true;
            this.Channel2Stop.Click += ChannelOperation_Click;
            // 
            // Channel0LoadedText
            // 
            this.Channel0LoadedText.AutoEllipsis = true;
            this.Channel0LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel0LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel0LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel0LoadedText.Location = new System.Drawing.Point(252, 441);
            this.Channel0LoadedText.Name = "Channel0LoadedText";
            this.Channel0LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel0LoadedText.TabIndex = 224;
            this.Channel0LoadedText.Text = "--NONE--";
            this.Channel0LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Channel1LoadedText
            // 
            this.Channel1LoadedText.AutoEllipsis = true;
            this.Channel1LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel1LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel1LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel1LoadedText.Location = new System.Drawing.Point(514, 441);
            this.Channel1LoadedText.Name = "Channel1LoadedText";
            this.Channel1LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel1LoadedText.TabIndex = 224;
            this.Channel1LoadedText.Text = "--NONE--";
            this.Channel1LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Channel2LoadedText
            // 
            this.Channel2LoadedText.AutoEllipsis = true;
            this.Channel2LoadedText.BackColor = System.Drawing.SystemColors.Window;
            this.Channel2LoadedText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Channel2LoadedText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.Channel2LoadedText.Location = new System.Drawing.Point(776, 441);
            this.Channel2LoadedText.Name = "Channel2LoadedText";
            this.Channel2LoadedText.Size = new System.Drawing.Size(256, 30);
            this.Channel2LoadedText.TabIndex = 224;
            this.Channel2LoadedText.Text = "--NONE--";
            this.Channel2LoadedText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLine
            // 
            this.timeLine.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.timeLine.Location = new System.Drawing.Point(12, 589);
            this.timeLine.Name = "timeLine";
            this.timeLine.Size = new System.Drawing.Size(1020, 52);
            this.timeLine.TabIndex = 214;
            this.timeLine.TabStop = false;
            this.timeLine.Text = "timeLine1";
            this.timeLine.StartTimeChanged += timeLine_StartTimeChanged;
            // 
            // Channel2Length
            // 
            this.Channel2Length.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel2Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel2Length.Highlighted = false;
            this.Channel2Length.InfoText = "End At: 59:50";
            this.Channel2Length.Location = new System.Drawing.Point(950, 551);
            this.Channel2Length.Name = "Channel2Length";
            this.Channel2Length.Size = new System.Drawing.Size(82, 32);
            this.Channel2Length.TabIndex = 117;
            this.Channel2Length.TabStop = false;
            this.Channel2Length.Tag = "";
            this.Channel2Length.Text = "--:--";
            this.Channel2Length.MouseDown += ChannelLength_MouseDown;
            // 
            // Channel1TimeLeft
            // 
            this.Channel1TimeLeft.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel1TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel1TimeLeft.Highlighted = false;
            this.Channel1TimeLeft.InfoText = "Remaining:";
            this.Channel1TimeLeft.Location = new System.Drawing.Point(599, 551);
            this.Channel1TimeLeft.Name = "Channel1TimeLeft";
            this.Channel1TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel1TimeLeft.TabIndex = 113;
            this.Channel1TimeLeft.TabStop = false;
            this.Channel1TimeLeft.Text = "0:00:00";
            // 
            // Channel1Length
            // 
            this.Channel1Length.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel1Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel1Length.Highlighted = false;
            this.Channel1Length.InfoText = "End At: 59:50";
            this.Channel1Length.Location = new System.Drawing.Point(687, 551);
            this.Channel1Length.Name = "Channel1Length";
            this.Channel1Length.Size = new System.Drawing.Size(82, 32);
            this.Channel1Length.TabIndex = 114;
            this.Channel1Length.TabStop = false;
            this.Channel1Length.Tag = "";
            this.Channel1Length.Text = "--:--";
            this.Channel1Length.MouseDown += ChannelLength_MouseDown;
            // 
            // trackList0
            // 
            this.trackList0.AllowDrop = true;
            this.trackList0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.trackList0.Channel = 0;
            this.trackList0.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList0.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackList0.LastIndexClicked = -1;
            this.trackList0.LoadedIndex = -1;
            this.trackList0.Location = new System.Drawing.Point(252, 12);
            this.trackList0.Name = "trackList0";
            this.trackList0.Size = new System.Drawing.Size(256, 397);
            this.trackList0.TabIndex = 10;
            this.trackList0.Text = "trackList0";
            this.trackList0.RequestChange += TrackList_RequestChange;
            // 
            // Channel0Length
            // 
            this.Channel0Length.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel0Length.HighlightColor = System.Drawing.Color.Red;
            this.Channel0Length.Highlighted = false;
            this.Channel0Length.InfoText = "End At: 59:50";
            this.Channel0Length.Location = new System.Drawing.Point(426, 551);
            this.Channel0Length.Name = "Channel0Length";
            this.Channel0Length.Size = new System.Drawing.Size(82, 32);
            this.Channel0Length.TabIndex = 111;
            this.Channel0Length.TabStop = false;
            this.Channel0Length.Tag = "";
            this.Channel0Length.Text = "--:--";
            this.Channel0Length.MouseDown += ChannelLength_MouseDown;
            // 
            // Channel2TimeLeft
            // 
            this.Channel2TimeLeft.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel2TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel2TimeLeft.Highlighted = false;
            this.Channel2TimeLeft.InfoText = "Remaining:";
            this.Channel2TimeLeft.Location = new System.Drawing.Point(863, 551);
            this.Channel2TimeLeft.Name = "Channel2TimeLeft";
            this.Channel2TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel2TimeLeft.TabIndex = 116;
            this.Channel2TimeLeft.TabStop = false;
            this.Channel2TimeLeft.Text = "0:00:00";
            // 
            // Channel0TimeLeft
            // 
            this.Channel0TimeLeft.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel0TimeLeft.HighlightColor = System.Drawing.Color.HotPink;
            this.Channel0TimeLeft.Highlighted = false;
            this.Channel0TimeLeft.InfoText = "Remaining:";
            this.Channel0TimeLeft.Location = new System.Drawing.Point(339, 551);
            this.Channel0TimeLeft.Name = "Channel0TimeLeft";
            this.Channel0TimeLeft.Size = new System.Drawing.Size(82, 32);
            this.Channel0TimeLeft.TabIndex = 110;
            this.Channel0TimeLeft.TabStop = false;
            this.Channel0TimeLeft.Text = "0:00:00";
            // 
            // Channel1TimeGone
            // 
            this.Channel1TimeGone.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel1TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel1TimeGone.Highlighted = false;
            this.Channel1TimeGone.InfoText = "Elapsed:";
            this.Channel1TimeGone.Location = new System.Drawing.Point(514, 551);
            this.Channel1TimeGone.Name = "Channel1TimeGone";
            this.Channel1TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel1TimeGone.TabIndex = 112;
            this.Channel1TimeGone.TabStop = false;
            this.Channel1TimeGone.Text = "0:00:00";
            // 
            // Channel2TimeGone
            // 
            this.Channel2TimeGone.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel2TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel2TimeGone.Highlighted = false;
            this.Channel2TimeGone.InfoText = "Elapsed:";
            this.Channel2TimeGone.Location = new System.Drawing.Point(776, 551);
            this.Channel2TimeGone.Name = "Channel2TimeGone";
            this.Channel2TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel2TimeGone.TabIndex = 115;
            this.Channel2TimeGone.TabStop = false;
            this.Channel2TimeGone.Text = "0:00:00";
            // 
            // Channel0TimeGone
            // 
            this.Channel0TimeGone.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.Channel0TimeGone.HighlightColor = System.Drawing.Color.Red;
            this.Channel0TimeGone.Highlighted = false;
            this.Channel0TimeGone.InfoText = "Elapsed:";
            this.Channel0TimeGone.Location = new System.Drawing.Point(252, 551);
            this.Channel0TimeGone.Name = "Channel0TimeGone";
            this.Channel0TimeGone.Size = new System.Drawing.Size(82, 32);
            this.Channel0TimeGone.TabIndex = 109;
            this.Channel0TimeGone.TabStop = false;
            this.Channel0TimeGone.Text = "0:00:00";
            // 
            // trackList2
            // 
            this.trackList2.AllowDrop = true;
            this.trackList2.Channel = 2;
            this.trackList2.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList2.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackList2.LastIndexClicked = -1;
            this.trackList2.LoadedIndex = -1;
            this.trackList2.Location = new System.Drawing.Point(776, 12);
            this.trackList2.Name = "trackList2";
            this.trackList2.Size = new System.Drawing.Size(256, 397);
            this.trackList2.TabIndex = 14;
            this.trackList2.Text = "trackList2";
            this.trackList2.RequestChange += TrackList_RequestChange;
            // 
            // trackList1
            // 
            this.trackList1.AllowDrop = true;
            this.trackList1.Channel = 1;
            this.trackList1.ContextMenuStrip = this.trackListContextMenuStrip;
            this.trackList1.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackList1.LastIndexClicked = -1;
            this.trackList1.LoadedIndex = -1;
            this.trackList1.Location = new System.Drawing.Point(514, 12);
            this.trackList1.Name = "trackList1";
            this.trackList1.Size = new System.Drawing.Size(256, 397);
            this.trackList1.TabIndex = 12;
            this.trackList1.Text = "trackList1";
            this.trackList1.RequestChange += TrackList_RequestChange;
            // 
            // trackTime1
            // 
            this.trackTime1.Channel = 1;
            this.trackTime1.CuePosition = 0;
            this.trackTime1.Duration = 0;
            this.trackTime1.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackTime1.IntroPosition = 0;
            this.trackTime1.Location = new System.Drawing.Point(514, 474);
            this.trackTime1.Name = "trackTime1";
            this.trackTime1.Position = 0;
            this.trackTime1.SilencePosition = 0;
            this.trackTime1.Size = new System.Drawing.Size(256, 72);
            this.trackTime1.TabIndex = 26;
            this.trackTime1.Text = "trackTime1";
            // 
            // trackTime2
            // 
            this.trackTime2.Channel = 2;
            this.trackTime2.CuePosition = 0;
            this.trackTime2.Duration = 0;
            this.trackTime2.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackTime2.IntroPosition = 0;
            this.trackTime2.Location = new System.Drawing.Point(776, 474);
            this.trackTime2.Name = "trackTime2";
            this.trackTime2.Position = 0;
            this.trackTime2.SilencePosition = 0;
            this.trackTime2.Size = new System.Drawing.Size(256, 72);
            this.trackTime2.TabIndex = 27;
            this.trackTime2.Text = "trackTime2";
            // 
            // trackTime0
            // 
            this.trackTime0.Channel = 0;
            this.trackTime0.CuePosition = 0;
            this.trackTime0.Duration = 0;
            this.trackTime0.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.trackTime0.IntroPosition = 0;
            this.trackTime0.Location = new System.Drawing.Point(252, 474);
            this.trackTime0.Name = "trackTime0";
            this.trackTime0.Position = 0;
            this.trackTime0.SilencePosition = 0;
            this.trackTime0.Size = new System.Drawing.Size(256, 72);
            this.trackTime0.TabIndex = 25;
            this.trackTime0.Text = "trackTime0";
            // 
            // BAPSPresenterMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1044, 737);
            this.ControlBox = false;
            this.Controls.Add(this.Channel2LoadedText);
            this.Controls.Add(this.Channel0LoadedText);
            this.Controls.Add(this.Channel1LoadedText);
            this.Controls.Add(this.Channel2Stop);
            this.Controls.Add(this.Channel1Stop);
            this.Controls.Add(this.Channel0Stop);
            this.Controls.Add(this.Channel2Pause);
            this.Controls.Add(this.Channel1Pause);
            this.Controls.Add(this.Channel0Pause);
            this.Controls.Add(this.Channel2Play);
            this.Controls.Add(this.Channel1Play);
            this.Controls.Add(this.loadShowButton);
            this.Controls.Add(this.Channel0Play);
            this.Controls.Add(this.Directory0Refresh);
            this.Controls.Add(this.bapsButton1);
            this.Controls.Add(this.Directory2Refresh);
            this.Controls.Add(this.Directory1Refresh);
            this.Controls.Add(this.timeLine);
            this.Controls.Add(this.MainTextDisplay);
            this.Controls.Add(this.Directory2);
            this.Controls.Add(this.Channel2Length);
            this.Controls.Add(this.Channel1TimeLeft);
            this.Controls.Add(this.Directory1);
            this.Controls.Add(this.Channel1Length);
            this.Controls.Add(this.Directory0);
            this.Controls.Add(this.trackList0);
            this.Controls.Add(this.Channel0Length);
            this.Controls.Add(this.Channel2TimeLeft);
            this.Controls.Add(this.Channel0TimeLeft);
            this.Controls.Add(this.Channel1TimeGone);
            this.Controls.Add(this.Channel2TimeGone);
            this.Controls.Add(this.Channel0TimeGone);
            this.Controls.Add(this.trackList2);
            this.Controls.Add(this.trackList1);
            this.Controls.Add(this.trackTime1);
            this.Controls.Add(this.trackTime2);
            this.Controls.Add(this.trackTime0);
            this.Font = (new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
                (byte)0));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BAPSPresenterMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BAPS Presenter";
            this.KeyDown += BAPSPresenterMain_KeyDown;
            this.trackListContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
#endregion

        private void trackList0_DragDrop(object sender, DragEventArgs e)
        {
        }
    }
}