﻿// This used to be BAPSPresenterMainReactions_system.cpp.

using System;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void addFileToDirectoryList(ushort directoryIndex, uint _fileIndex, string entry)
        {
            if (DirectoryOutOfBounds(directoryIndex)) return;
            /** Add the new entry to the bottom of the listbox **/
            directoryList[directoryIndex].Items.Add(entry);
        }

        private void clearFiles(ushort directoryIndex, string niceDirectoryName)
        {
            if (DirectoryOutOfBounds(directoryIndex)) return;
            /** Empty the list box ready for new entries (required due to implicit indexing) **/
            directoryList[directoryIndex].Items.Clear();
            directoryRefresh[directoryIndex].Text = niceDirectoryName;
        }

        private void displayVersion(string version, string date, string time, string author)
        {
            if (about == null) return;
            about.Invoke((Action<string, string, string, string>)about.serverVersion, version, date, time, author);
        }
    }
}