// This used to be BAPSPresenterMainReactions_database.cpp.

using BAPSClientCommon;
using System;

namespace BAPSPresenter2
{
    partial class Main
    {
        private void SetupDatabaseReactions(Receiver r)
        {
            r.LibraryResult += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<uint, int, string>)addLibraryResult, e.resultID, (int)e.dirtyStatus, e.description);
                }
                else addLibraryResult(e.resultID, e.dirtyStatus, e.description);
            };
            r.ShowResult += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<uint, string>)addShowResult, e.showID, e.description);
                }
                else addShowResult(e.showID, e.description);
            };
            r.ListingResult += (sender, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke((Action<uint, uint, string>)addListingResult, e.listingID, e.channelID, e.description);
                }
                else addListingResult(e.listingID, e.channelID, e.description);
            };
        }

        /** Just pass all the data through to the child form (if present) **/

        private void addLibraryResult(uint index, int dirtyStatus, string result)
        {
            if (recordLibrarySearch == null) return;
            recordLibrarySearch.Invoke((Action<object, object, string>)recordLibrarySearch.add, (int)index, dirtyStatus, result);
        }

        private void setLibraryResultCount(int count)
        {
            if (recordLibrarySearch == null) return;
            recordLibrarySearch.Invoke((Action<object>)recordLibrarySearch.setResultCount, count);
        }

        private void notifyLibraryError(int errorcode, string description)
        {
            if (recordLibrarySearch == null) return;
            recordLibrarySearch.Invoke((Action<object, string>)recordLibrarySearch.handleError, errorcode, description);
        }

        private void addShowResult(uint showid, string description)
        {
            if (loadShowDialog == null) return;
            loadShowDialog.Invoke((Action<object, string>)loadShowDialog.addShow, (int)showid, description);
        }

        private void setShowResultCount(int count)
        {
            if (loadShowDialog == null) return;
            loadShowDialog.Invoke((Action<object>)loadShowDialog.setShowResultCount, count);
        }

        private void addListingResult(uint listingid, uint channel, string description)
        {
            if (loadShowDialog == null) return;
            loadShowDialog.Invoke((Action<object, object, string>)loadShowDialog.addListing, (int)listingid, (int)channel, description);
        }

        private void setListingResultCount(int count)
        {
            if (loadShowDialog == null) return;
            loadShowDialog.Invoke((Action<object>)loadShowDialog.setListingResultCount, count);
        }

        private void notifyLoadShowError(int errorCode, string message)
        {
            if (loadShowDialog == null) return;
            loadShowDialog.Invoke((Action<object, string>)loadShowDialog.notifyError, errorCode, message);
        }
    }
}