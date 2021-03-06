﻿// This used to be BAPSPresenterMainReactions_system.cpp.

using System;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Updaters;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupSystemReactions(IServerUpdater r)
        {
            r.ObserveDirectoryFileAdd += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.DirectoryFileAddEventArgs>)addFileToDirectoryList, sender, e);
                }
                else addFileToDirectoryList(sender, e);
            };
            r.ObserveDirectoryPrepare += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((EventHandler<Updates.DirectoryPrepareEventArgs>)clearFiles, sender, e);
                }
                else clearFiles(sender, e);
            };
            r.ObserveVersion += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<string, string, string, string>)displayVersion, e.Version, e.Date, e.Time, e.Author);
                }
                else displayVersion(e.Version, e.Date, e.Time, e.Author);
            };
            r.ObserveTextScroll += (sender, e) =>
            {
                var td = textDialog;
                if (td == null) return;
                if (td.InvokeRequired)
                {
                    td.Invoke((Action<int>)td.scroll, (int)e);
                }
                else td.textSize((int)e);
            };
            r.ObserveTextSizeChange += (sender, e) =>
            {
                var td = textDialog;
                if (td == null) return;
                if (td.InvokeRequired)
                {
                    td.Invoke((Action<int>)td.textSize, (int)e);
                }
                else td.textSize((int)e);
            };
            r.ObserveServerQuit += (sender, e) =>
            {
                var description = "The server is shutting down/restarting.";

                if (InvokeRequired)
                {
                    Invoke((Action<string, bool>)SendQuit, description, e);
                }
                else SendQuit(description, e);
            };
        }

        private void addFileToDirectoryList(object sender, Updates.DirectoryFileAddEventArgs e)
        {
            if (DirectoryOutOfBounds(e.DirectoryId)) return;
            // TODO(@MattWindsor91): file index?
            /** Add the new entry to the bottom of the listbox **/
            _directories[e.DirectoryId].Add(e.Description);
        }

        private void clearFiles(object sender, Updates.DirectoryPrepareEventArgs e)
        {
            if (DirectoryOutOfBounds(e.DirectoryId)) return;
            /** Empty the list box ready for new entries (required due to implicit indexing) **/
            _directories[e.DirectoryId].Clear(e.Name);
        }

        private void displayVersion(string version, string date, string time, string author)
        {
            if (about == null) return;
            about.Invoke((Action<string, string, string, string>)about.serverVersion, version, date, time, author);
        }
    }
}