using System;
using System.Collections.ObjectModel;
using BAPSClientCommon;
using BAPSClientCommon.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     The view model for a directory.
    /// </summary>
    public class DirectoryViewModel : ViewModelBase
    {
        private string _name;

        public DirectoryViewModel(ushort directoryId)
        {
            DirectoryId = directoryId;
        }

        public ushort DirectoryId { get; }

        /// <summary>
        ///     The human-readable name of this directory.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        public ObservableCollection<DirectoryEntry> Files { get; } = new ObservableCollection<DirectoryEntry>();

        public void Register(IMessenger m)
        {
            m.Register(this, (Action<ServerUpdates.DirectoryFileAddArgs>) HandleDirectoryFileAdd);
            m.Register(this, (Action<ServerUpdates.DirectoryPrepareArgs>) HandleDirectoryPrepare);
        }

        private void HandleDirectoryFileAdd(ServerUpdates.DirectoryFileAddArgs e)
        {
            if (e.DirectoryId != DirectoryId) return;
            Files.Insert((int) e.Index, new DirectoryEntry(DirectoryId, e.Description));
        }

        /// <summary>
        ///     Handles a directory-prepare server update from the messenger bus.
        ///     <para>
        ///         Note that the main view model also listens for directory-prepare
        ///         updates; this is so that it can create
        ///         <see cref="DirectoryViewModel" />s for directory IDs that haven't
        ///         previously been used.
        ///     </para>
        /// </summary>
        /// <param name="e">The server update payload</param>
        private void HandleDirectoryPrepare(ServerUpdates.DirectoryPrepareArgs e)
        {
            if (e.DirectoryId != DirectoryId) return;
            Files.Clear();
            Name = e.Name;
        }
    }
}