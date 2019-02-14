using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Abstract base class providing the parts of <see cref="ITrackListViewModel"/>
    ///     that are largely the same across implementations.
    /// </summary>
    public abstract class TrackListViewModelBase : ChannelComponentViewModelBase, ITrackListViewModel
    {
        [CanBeNull] private RelayCommand _deleteItemCommand;
        [CanBeNull] private RelayCommand _resetPlaylistCommand;
        [CanBeNull] private RelayCommand<int> _loadTrackCommand;
        
        public abstract int SelectedIndex { get; set; }
        
        public RelayCommand ResetPlaylistCommand => _resetPlaylistCommand
                                                    ?? (_resetPlaylistCommand =
                                                        new RelayCommand(ResetPlaylist, CanResetPlaylist));

        public RelayCommand DeleteItemCommand => _deleteItemCommand
                                                 ?? (_deleteItemCommand = new RelayCommand(DeleteItem, CanDeleteItem));

        public RelayCommand<int> LoadTrackCommand => _loadTrackCommand
                                                     ?? (_loadTrackCommand =
                                                         new RelayCommand<int>(LoadTrack, CanLoadTrack));
        
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
        
        public ObservableCollection<TrackViewModel> Tracks { get; } = new ObservableCollection<TrackViewModel>();

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
        
        public TrackViewModel TrackAt(int index)
        {
            return Tracks.ElementAtOrDefault(index) ?? TrackViewModel.MakeNull();
        }
        
        public void DragOver(IDropInfo dropInfo)
        {
            switch (dropInfo.Data)
            {
                case DirectoryEntry _:
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
            }
        }

        /// <summary>
        ///     Handles an attempt to drop a directory entry into this track-list.
        /// </summary>
        /// <param name="entry">The entry to handle.</param>
        protected abstract void DropDirectoryEntry(DirectoryEntry entry);
        
        protected TrackListViewModelBase(ushort channelId) : base(channelId)
        {
        }
    }
}