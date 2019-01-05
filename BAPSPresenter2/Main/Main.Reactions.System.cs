// This used to be BAPSPresenterMainReactions_system.cpp.

using BAPSCommon;
using System;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupSystemReactions(Receiver r)
        {
            r.DirectoryFileAdd += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, uint, string>)addFileToDirectoryList, e.directoryID, e.index, e.description);
                }
                else addFileToDirectoryList(e.directoryID, e.index, e.description);
            };
            r.DirectoryPrepare += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<ushort, string>)clearFiles, e.directoryID, e.directoryName);
                }
                else clearFiles(e.directoryID, e.directoryName);
            };
            r.Version += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<string, string, string, string>)displayVersion, e.Version, e.Date, e.Time, e.Author);
                }
                else displayVersion(e.Version, e.Date, e.Time, e.Author);
            };
            r.TextScroll += (sender, e) =>
            {
                var td = textDialog;
                if (td == null) return;
                if (td.InvokeRequired)
                {
                    td.Invoke((Action<int>)td.scroll, (int)e);
                }
                else td.textSize((int)e);
            };
            r.TextSizeChange += (sender, e) =>
            {
                var td = textDialog;
                if (td == null) return;
                if (td.InvokeRequired)
                {
                    td.Invoke((Action<int>)td.textSize, (int)e);
                }
                else td.textSize((int)e);
            };
            r.ServerQuit += (sender, e) =>
            {
                var description = "The server is shutting down/restarting.";

                if (InvokeRequired)
                {
                    Invoke((Action<string, bool>)SendQuit, description, e);
                }
                else SendQuit(description, e);
            };
        }

        private void addFileToDirectoryList(ushort directoryIndex, uint _fileIndex, string entry)
        {
            if (DirectoryOutOfBounds(directoryIndex)) return;
            // TODO(@MattWindsor91): file index?
            /** Add the new entry to the bottom of the listbox **/
            bapsDirectories[directoryIndex].Add(entry);
        }

        private void clearFiles(ushort directoryIndex, string niceDirectoryName)
        {
            if (DirectoryOutOfBounds(directoryIndex)) return;
            /** Empty the list box ready for new entries (required due to implicit indexing) **/
            bapsDirectories[directoryIndex].Clear(niceDirectoryName);
        }

        private void displayVersion(string version, string date, string time, string author)
        {
            if (about == null) return;
            about.Invoke((Action<string, string, string, string>)about.serverVersion, version, date, time, author);
        }
    }
}