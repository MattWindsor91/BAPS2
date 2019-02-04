using BAPSClientCommon;
using BAPSPresenter;
using System;
using System.Windows.Forms;
using BAPSClientCommon.BapsNet;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using Message = BAPSClientCommon.BapsNet.Message;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void TimelineChanged(object sender, TimelineChangeEventArgs e)
        {
            switch (e.ChangeType)
            {
                case TimelineChangeType.Start:
                    timeLine.UpdateStartTime(e.ChannelID, e.Value);
                    break;
                case TimelineChangeType.Duration:
                    timeLine.UpdateDuration(e.ChannelID, e.Value);
                    break;
                case TimelineChangeType.Position:
                    timeLine.UpdatePosition(e.ChannelID, e.Value);
                    break;
                default:
                    throw new NotImplementedException("unexpected timeline change type");
            }
        }

        /** functions to receive events from the custom TrackTime class */
        private void HandlePositionChanged(object sender, Requests.MarkerEventArgs e)
        {
            var cmd = Command.Playback | e.Marker.AsCommand() | (Command)e.ChannelId;
            _core.SendAsync(new Message(cmd).Add(e.NewValue));
        }

        /** functions to receive context menu events **/
        private void ChannelListClear_Click(object sender, EventArgs e)
        {
            var mi = (MenuItem)sender;
            var cm = (ContextMenu)mi.Parent;
            var tl = (TrackList)cm.SourceControl;
            var cmd = Command.Playlist | Command.ResetPlaylist | (Command)tl.Channel;
            _core.SendAsync(new Message(cmd));
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
                    cmd = Command.Playback | Command.Play | 0;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F2: /** F2 Pause **/
                    cmd = Command.Playback | Command.Pause | 0;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F3: /** F3 Stop **/
                    cmd = Command.Playback | Command.Stop | 0;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F4 | Keys.Alt:
                    {
                        e.Handled = true;
                    }
                    break;

                /** Keys F5-F8 are for channel 1 **/
                case Keys.F5: /** F5 Play **/
                    cmd = Command.Playback | Command.Play | (Command)1;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F6: /** F6 Pause **/
                    cmd = Command.Playback | Command.Pause | (Command)1;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F7: /** F7 Stop **/
                    cmd = Command.Playback | Command.Stop | (Command)1;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;

                /** Keys F9-F12 are for channel 2 **/
                case Keys.F9: /** F9 Play **/
                    cmd = Command.Playback | Command.Play | (Command)2;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F10: /** F10 Pause **/
                    cmd = Command.Playback | Command.Pause | (Command)2;
                    _core.SendAsync(new Message(cmd));
                    e.Handled = true;
                    break;
                case Keys.F11: /** F11 Stop **/
                    cmd = Command.Playback | Command.Stop | (Command)2;
                    _core.SendAsync(new Message(cmd));
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
                    AlterWindow();
                    e.Handled = true;
                    break;
                case KeyShortcuts.LocalConfig:
                    OpenLocalConfigDialog();
                    e.Handled = true;
                    break;
                case KeyShortcuts.Config:
                    OpenConfigDialog();
                    e.Handled = true;
                    break;
                case KeyShortcuts.Security:
                    OpenSecurityDialog();
                    e.Handled = true;
                    break;
                case KeyShortcuts.TextMaximised:
                    ToggleTextDialog(true);
                    e.Handled = true;
                    break;
                case KeyShortcuts.Text:
                    ToggleTextDialog(false);
                    e.Handled = true;
                    break;
                case KeyShortcuts.About:
                    OpenAboutDialog();
                    e.Handled = true;
                    break;
            }
        }

        private void OpenSecurityDialog()
        {
            using (securityDialog = new Dialogs.Security(_core))
            {
                securityDialog.KeyDownForward += BAPSPresenterMain_KeyDown;
                _core.SendAsync(new Message(Command.Config | Command.GetPermissions));
                securityDialog.ShowDialog();
            }
            securityDialog = null;
        }

        private void OpenAboutDialog()
        {
            using (about = new Dialogs.About())
            {
                about.KeyDownForward += BAPSPresenterMain_KeyDown;
                _core.SendAsync(new Message(Command.System | Command.Version));
                about.ShowDialog();
            }
            about = null;
        }

        private void OpenConfigDialog()
        {
            using (configDialog = new Dialogs.Config(_core))
            {
                configDialog.KeyDownForward += BAPSPresenterMain_KeyDown;
                configDialog.ShowDialog();
            }
            configDialog = null;
        }

        private void OpenLocalConfigDialog()
        {
            using (var ccd = new Dialogs.LocalConfig())
            {
                ccd.KeyDownForward += BAPSPresenterMain_KeyDown;
                ccd.ShowDialog();
            }
            /** Enable or disable the timers depending on the config setting **/
            bool enableTimers = ConfigManager.getConfigValueString("EnableTimers", "Yes") == "Yes";
            EnableTimerControls(enableTimers);
        }

        private void ToggleTextDialog(bool shouldMaximize)
        {
            bool wasOpen = true;
            if (!textDialog.Visible)
            {
                wasOpen = false;
                textDialog.Show();
                textDialog.Invoke((Action<string>)textDialog.updateText, MainTextDisplay.Text);
            }
            if (shouldMaximize)
            {
                textDialog.Invoke((Action)textDialog.toggleMaximize);
            }
            else if (wasOpen)
            {
                textDialog.Hide();
            }
        }

        private void AlterWindow()
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
        }

        private void RefreshDirectory(object sender, ushort channel)
        {
            var cmd = Command.System | Command.ListFiles;
            cmd |= (Command)channel & (Command)0x3f;
            _core.SendAsync(new Message(cmd));
        }

        private void SearchRecordLib_Click(object sender, EventArgs e)
        {
#if false // TODO(@MattWindsor91): port this
	        /** We have a handle stored in the class so that data can be passed to the object **/
	        recordLibrarySearch = new RecordLibrarySearch(this, core.SendQueue);
	        recordLibrarySearch.ShowDialog();
	        /** Always get rid of it again **/
	        recordLibrarySearch.Dispose();
        	recordLibrarySearch = null;
#endif
        }

        private void loadShow_Click(object sender, EventArgs e)
        {
#if false // TODO(@MattWindsor91): port this
            /** Handle stored for same reason as record library **/
            loadShowDialog = new LoadShowDialog(this, core.SendQueue);
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

        private void HandleChannelStateRequest(object sender, Updates.PlayerStateEventArgs e)
        {
            _controllers.ControllerFor(e.ChannelId).SetState(e.State);
        }

        private void HandleItemDeleteRequest(object sender, Updates.TrackDeleteEventArgs e)
        {
            _controllers.ControllerFor(e.ChannelId).DeleteItemAt(e.Index);
        }

        internal void TrackList_RequestChange(object sender, RequestChangeEventArgs e)
        {
            var channelId = e.channel & (ushort)Command.ChannelMask;
            var controller = _controllers.ControllerFor((ushort)channelId);

            switch (e.ct)
            {
                case ChangeType.SELECTEDINDEX:
                    // This won't be an impossible load---they're filtered out in BAPSChannel.
                    controller.Select((uint)e.index);
                    break;
                case ChangeType.MOVEINDEX:
                    {
                        var cmd = Command.Playlist | (Command)(e.channel & 0x3f) | Command.MoveItemTo;
                        _core.SendAsync(new Message(cmd).Add((uint)e.index).Add((uint)e.index2));
                    }
                    break;
                case ChangeType.DELETEINDEX:
                    {
                        var cmd = Command.Playlist | (Command)(e.channel & 0x3f) | Command.DeleteItem;
                        _core.SendAsync(new Message(cmd).Add((uint)e.index));
                    }
                    break;
                case ChangeType.ADD:
                    {
                        var cmd = Command.Playlist | Command.AddItem | (Command)(e.channel & 0x3f);
                        _core.SendAsync(new Message(cmd).Add((uint)TrackType.File).Add((uint)e.index).Add(_directories[e.index].TrackAt(e.index2)));
                    }
                    break;
                case ChangeType.COPY:
                    {
                        var cmd = Command.Playlist | Command.CopyItem | (Command)(e.channel & 0x3f);
                        _core.SendAsync(new Message(cmd).Add((uint)e.index).Add((uint)e.index2));
                    }
                    break;
            }
        }

        private void HandleChannelConfigChange(object sender, ChannelConfigChangeArgs e) =>
            _controllers.ControllerFor(e.ChannelId).Configure(e.Type);


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
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == BAPSChannelplayOnLoadToolStripMenuItem)
            {
                var oci = ConfigCache.getOption("Play on load");
                var newSetting = "Yes";
                if (playOnLoadToolStripMenuItem.Checked)
                {
                    newSetting = "No";
                }
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList[newSetting]));
            }
            else if (e.ClickedItem == repeatNoneToolStripMenuItem && !repeatNoneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["No repeat"]));
            }
            else if (e.ClickedItem == repeatOneToolStripMenuItem && !repeatOneToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["Repeat one"]));
            }
            else if (e.ClickedItem == repeatAllToolStripMenuItem && !repeatAllToolStripMenuItem.Checked)
            {
                var oci = ConfigCache.getOption("Repeat");
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)oci.optionid, (uint)oci.type, (uint)(int)oci.choiceList["Repeat all"]));
            }
            else if (e.ClickedItem == deleteItemToolStripMenuItem)
            {
                cmd = Command.PLAYLIST | Command.DELETEITEM | (Command)tl.Channel;
                core.SendAsync(new BAPSClientCommon.Message(cmd).Add(, (uint)tl.LastIndexClicked));
            }
            else if (e.ClickedItem == resetChannelStripMenuItem)
            {
                cmd = Command.PLAYLIST | Command.RESETPLAYLIST | (Command)tl.Channel;
                core.SendAsync(new BAPSClientCommon.Message(cmd));
            }
            else if (e.ClickedItem == showAudioWallToolStripMenuItem)
            {
                OpenAudioWall(tl);
            }
        }
        #endif
        
        private void countdownTick(object sender, EventArgs e)
        {
            if (_timersEnabled)
            {
                foreach (var chan in _channels) chan.CountdownTick();
            }
            timeLine.Tick();
        }

        private void timeLine_StartTimeChanged(object sender, BAPSFormControls.TimeLine.TimeLineEventArgs e)
        {
            if (!_timersEnabled) return;
            _channels[e.channel].UpdateCountDown(e.startTime / 1000 % 3600);
        }
    }
}
