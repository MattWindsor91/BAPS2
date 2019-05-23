using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Abstract base class for directory view models.
    /// </summary>
    public abstract class DirectoryViewModelBase : ViewModelBase, IDirectoryViewModel
    {
        private RelayCommand _refreshCommand;

        protected DirectoryViewModelBase(ushort directoryId)
        {
            DirectoryId = directoryId;
        }

        /// <summary>
        ///     The human-readable name of this directory.
        /// </summary>
        public abstract string Name { get; protected set; }

        /// <summary>
        ///     The numeric server-side identifier of this directory.
        /// </summary>
        public ushort DirectoryId { get; }

        /// <summary>
        ///     A command that, when activated, sends a refresh request to the server.
        /// </summary>
        [NotNull]
        public RelayCommand RefreshCommand => _refreshCommand
                                              ?? (_refreshCommand = new RelayCommand(
                                                  Refresh,
                                                  CanRefresh
                                              ));

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        public ObservableCollection<DirectoryEntry> Files { get; } = new ObservableCollection<DirectoryEntry>();

        /// <summary>
        ///     Executes a directory refresh command.
        /// </summary>
        protected abstract void Refresh();

        /// <summary>
        ///     Gets whether a directory refresh can happen
        /// </summary>
        /// <returns>Whether <see cref="Refresh" /> can fire.</returns>
        protected abstract bool CanRefresh();

        public abstract void Dispose();
    }
}