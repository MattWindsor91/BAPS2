namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Abstract base class providing the parts of <see cref="ITrackListViewModel" />
    ///     that are largely the same across implementations.
    /// </summary>
    public abstract class TrackListViewModelBase : ChannelComponentViewModelBase, ITrackListViewModel
    {
        [CanBeNull] private RelayCommand _deleteItemCommand;
        [CanBeNull] private RelayCommand<int> _loadTrackCommand;
        [CanBeNull] private RelayCommand _resetPlaylistCommand;

        protected TrackListViewModelBase(ushort channelId) : base(channelId)
        {
        }

        public abstract int SelectedIndex { get; set; }

        public RelayCommand ResetPlaylistCommand => _resetPlaylistCommand
                                                    ?? (_resetPlaylistCommand =
                                                        new RelayCommand(ResetPlaylist, CanResetPlaylist));

        public RelayCommand DeleteItemCommand => _deleteItemCommand
                                                 ?? (_deleteItemCommand = new RelayCommand(DeleteItem, CanDeleteItem));

        public RelayCommand<int> LoadTrackCommand => _loadTrackCommand
                                                     ?? (_loadTrackCommand =
                                                         new RelayCommand<int>(LoadTrack, CanLoadTrack));

        public ObservableCollection<TrackViewModel> Tracks { get; } = new ObservableCollection<TrackViewModel>();

        public TrackViewModel TrackAt(int index)
        {
            return Tracks.ElementAtOrDefault(index) ?? TrackViewModel.MakeNull();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            switch (dropInfo.Data)
            {
                case DirectoryEntry _:
                case DataObject d when d.ContainsText():
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Copy;
                    break;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            switch (dropInfo.Data)
            {
                case DirectoryEntry dirEntry:
                    DropDirectoryEntry(dirEntry);
                    break;
                case DataObject d when d.ContainsText():
                    DropText(d.GetText());
                    break;
            }
        }

        /// <summary>
        ///     Checks whether the load-track command can fire.
        /// </summary>
        /// <param name="index">The index being loaded.</param>
        /// <returns>Whether <see cref="LoadTrackCommand" /> can fire.</returns>
        protected abstract bool CanLoadTrack(int index);

        /// <summary>
        ///     Asks the server to load the track with the given index.
        /// </summary>
        /// <param name="index">The index being loaded.</param>
        protected abstract void LoadTrack(int index);

        /// <summary>
        ///     Checks whether the reset-playlist command can fire.
        /// </summary>
        /// <returns>Whether <see cref="ResetPlaylistCommand" /> can fire.</returns>
        protected abstract bool CanResetPlaylist();

        /// <summary>
        ///     Resets the channel's playlist.
        /// </summary>
        protected abstract void ResetPlaylist();

        /// <summary>
        ///     Checks whether the delete-item command can fire.
        /// </summary>
        /// <returns>Whether <see cref="DeleteItemCommand" /> can fire.</returns>
        protected abstract bool CanDeleteItem();

        /// <summary>
        ///     Deletes the currently selected item.
        /// </summary>
        protected abstract void DeleteItem();

        /// <summary>
        ///     Handles an attempt to drop a directory entry into this track-list.
        /// </summary>
        /// <param name="entry">The entry to handle.</param>
        protected abstract void DropDirectoryEntry(DirectoryEntry entry);

        /// <summary>
        ///     Handles an attempt to drop some text into this track-list.
        /// </summary>
        /// <param name="text">The text to handle.</param>
        protected abstract void DropText(string text);

        public abstract void Dispose();
    }
}