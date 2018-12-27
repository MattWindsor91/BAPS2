using BAPSPresenter;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    partial class Main
    {

        /** functions to receive events from the custom TrackTime class */
        private void positionChanged(ushort channel, int position)
        {
            var cmd = Command.PLAYBACK | Command.POSITION | (Command)channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)position));
        }

        private void cuePositionChanged(ushort channel, int cuePosition)
        {
            var cmd = Command.PLAYBACK | Command.CUEPOSITION | (Command)channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)cuePosition));
        }

        private void introPositionChanged(ushort channel, int introPosition)
        {
            var cmd = Command.PLAYBACK | Command.INTROPOSITION | (Command)channel;
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, (uint)introPosition));
        }

        /** functions to receive context menu events **/
        private void ChannelListClear_Click(object sender, EventArgs e)
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
            switch (e.KeyData)
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
                case Keys.F4 | Keys.Alt:
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
                case KeyShortcuts.Shutdown:
                    {
                        var result = MessageBox.Show("Do you wish to disconnect?", "Shutdown?", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            Close();
                        }
                        e.Handled = true;
                    }
                    break;
                case KeyShortcuts.AlterWindow:
                    {
                        if (WindowState == FormWindowState.Normal)
                        {
                            System.Drawing.Rectangle bounds = Screen.GetWorkingArea(this);
                            bounds.X = 0;
                            bounds.Y = 0;
                            MaximizedBounds = bounds;
                            MaximumSize = bounds.Size;
                            FormBorderStyle = FormBorderStyle.None;
                            WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            FormBorderStyle = FormBorderStyle.FixedSingle;
                            WindowState = FormWindowState.Normal;
                        }
                        e.Handled = true;
                    }
                    break;
                case KeyShortcuts.LocalConfig:
                    {
                        LocalConfigDialog ccd = new LocalConfigDialog(this);
                        ccd.ShowDialog();
                        /** Enable or disable the timers depending on the config setting **/
                        bool enableTimers = string.Compare(ConfigManager.getConfigValueString("EnableTimers", "Yes"), "Yes") == 0;
                        enableTimerControls(enableTimers);
                        e.Handled = true;
                    }
                    break;
                case KeyShortcuts.Config:
                    {
#if false
                        configDialog = new ConfigDialog(this, msgQueue);
                        configDialog.ShowDialog();
                        delete configDialog;
                        configDialog = nullptr;
#endif
                        e.Handled = true;
                    }
                    break;
                case KeyShortcuts.Security:
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
                case KeyShortcuts.TextMaximised:
                case KeyShortcuts.Text:
                    {
                        bool wasOpen = true;
                        if (!textDialog.Visible)
                        {
                            wasOpen = false;
                            textDialog.Show();
                            textDialog.Invoke((Action<string>)textDialog.updateText, MainTextDisplay.Text);
                        }
                        if (e.Shift)
                        {
                            textDialog.Invoke((Action)textDialog.toggleMaximize);
                        }
                        else if (wasOpen)
                        {
                            textDialog.Hide();
                        }
                        e.Handled = true;
                    }
                    break;
                case KeyShortcuts.About:
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

        private void TrackBar_Scroll(ushort channel, uint value)
        {
            /** Update the server with the new value the user has selected **/
            Command cmd = Command.PLAYBACK | Command.POSITION;
            cmd |= ((Command)channel) & (Command)0x3f;
            /** The scrollbar value is the new setting **/
            msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, value));
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

        private void ChannelOperation_Click(ChannelOperationLookup col)
        {
            var cmd = Command.PLAYBACK | (Command)col.co | (Command)(col.channel & 0x3f);
            msgQueue.Enqueue(new ActionMessage((ushort)cmd));
        }

        internal void TrackList_RequestChange(ushort channelID, RequestChangeEventArgs e)
        {
            switch ((ChangeType)e.ct)
            {
                case ChangeType.SELECTEDINDEX:
                    {
                        // This won't be an impossible load---they're filtered out in BAPSChannel.
                        var cmd = Command.PLAYBACK | Command.LOAD;
                        cmd |= (Command)(channelID & 0x3f);
                        /** Get the selected index as above **/
                        var intArg = (uint)e.index;
                        msgQueue.Enqueue(new ActionMessageU32int((ushort)cmd, intArg));
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

#if false // @MattWindsor91
        private void trackListContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tl = (TrackList)sender;
            Debug.Assert(tl != null, "menu strip doesn't have a source control!");

            var cmd = Command.CONFIG | Command.SETCONFIGVALUE | Command.CONFIG_USEVALUEMASK | (Command)tl.Channel;
            if (e.ClickedItem == BAPSChannel.automaticAdvanceToolStripMenuItem)
            {
                var oci = ConfigCache.getOption("Automatically advance");
                var newSetting = "Yes";
                if (BAPSChannelautomaticAdvanceToolStripMenuItem.Checked)
                {
                    newSetting = "No";
                }
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == BAPSChannelplayOnLoadToolStripMenuItem)
            {
                var oci = ConfigCache.getOption("Play on load");
                var newSetting = "Yes";
                if (playOnLoadToolStripMenuItem.Checked)
                {
                    newSetting = "No";
                }
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == repeatNoneToolStripMenuItem && !repeatNoneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["No repeat"]));
            }
            else if (e.ClickedItem == repeatOneToolStripMenuItem && !repeatOneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["Repeat one"]));
            }
            else if (e.ClickedItem == repeatAllToolStripMenuItem && !repeatAllToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                msgQueue.Enqueue(new ActionMessageU32intU32intU32int((ushort)cmd, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["Repeat all"]));
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
                OpenAudioWall(tl);
            }
        }
        #endif
        
        private void countdownTick(object sender, EventArgs e)
        {
            if (timersEnabled)
            {
                foreach (var chan in bapsChannels) chan.CountdownTick();
            }
            timeLine.tick();
        }

        private void timeLine_StartTimeChanged(object sender, TimeLineEventArgs e)
        {
            if (!timersEnabled) return;
            bapsChannels[e.channel].UpdateCountDown(e.startTime / 1000 % 3600);
        }
    }
}
