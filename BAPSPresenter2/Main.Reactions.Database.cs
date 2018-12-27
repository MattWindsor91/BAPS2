// This used to be BAPSPresenterMainReactions_database.cpp.

using System;

namespace BAPSPresenter2
{
    partial class Main
    {
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