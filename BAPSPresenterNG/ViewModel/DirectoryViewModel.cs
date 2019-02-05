using System;
using System.Collections.ObjectModel;
using BAPSClientCommon;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     The view model for a directory.
    /// </summary>
    public class DirectoryViewModel : ViewModelBase, IDisposable
    {
        private string _name;
        private readonly IServerUpdater _updater;

        public DirectoryViewModel(ushort directoryId, [CanBeNull] IServerUpdater updater)
        {
            _updater = updater;
            DirectoryId = directoryId;
            RegisterForServerUpdates();
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

        private void RegisterForServerUpdates()
        {
            _updater.DirectoryFileAdd += HandleDirectoryFileAdd;
            _updater.DirectoryPrepare += HandleDirectoryPrepare;
        }

        private void UnregisterForServerUpdates()
        {
            _updater.DirectoryFileAdd -= HandleDirectoryFileAdd;
            _updater.DirectoryPrepare -= HandleDirectoryPrepare;
        }

        private void HandleDirectoryFileAdd(object sender, Updates.DirectoryFileAddArgs e)
        {
            if (e.DirectoryId != DirectoryId) return;
            var entry = new DirectoryEntry(DirectoryId, e.Description);
            DispatcherHelper.CheckBeginInvokeOnUI(() => Files.Insert((int) e.Index, entry));
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
        /// <param name="sender">Ignored.</param>
        /// <param name="e">The server update payload.</param>
        private void HandleDirectoryPrepare(object sender, Updates.DirectoryPrepareArgs e)
        {
            if (e.DirectoryId != DirectoryId) return;
            Name = e.Name;
            DispatcherHelper.CheckBeginInvokeOnUI(Files.Clear);
        }

        public void Dispose()
        {
            UnregisterForServerUpdates();
        }
    }
}