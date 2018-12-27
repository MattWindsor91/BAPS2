using BAPSPresenter;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    partial class Main
    {

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
        public void BAPSPresenterMain_KeyDown(object sender, KeyEventArgs e)
        {
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
                        msgQueue.Enqueue(new ActionMessage((ushort)(Command.SYSTEM | Command.VERSION)));
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

        private void TrackBar_Scroll(object sender, System.EventArgs e)
        {
            var trackBar = (TrackBar)sender;
            /** Update the server with the new value the user has selected **/
            Command cmd = Command.PLAYBACK | Command.POSITION;
            /** The tag contains the channel number as a managed int **/
            cmd |= ((Command)trackBar.Tag) & (Command)0x3f;
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
                            ((ChannelTimeoutStruct)(loadImpossibleTimer[e.channel].Tag)).timeout = 10;
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
    }
}
