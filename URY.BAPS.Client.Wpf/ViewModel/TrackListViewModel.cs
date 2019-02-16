using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public class TrackListViewModel : TrackListViewModelBase, IDisposable
    {
        private int _selectedIndex = -1;

        public TrackListViewModel(ushort channelId, [CanBeNull] ChannelController controller) : base(channelId)
        {
            Controller = controller;

            SubscribeToServerUpdates();
        }

        #region Subscriptions
        
        /// <summary>
        ///     The list of handles to observable subscriptions that this view model creates.
        /// </summary>
        [NotNull] private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();
        
        private void SubscribeToServerUpdates()
        {
            var playbackUpdater = Controller.PlaybackUpdater;
             // NB: the Player also registers this event.
            _subscriptions.Add(OnThisChannel(playbackUpdater.ObserveTrackLoad).Subscribe(HandleTrackLoad));
            
            var playlistUpdater = Controller.PlaylistUpdater;
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackAdd).Subscribe(HandleItemAdd));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackMove).Subscribe(HandleItemMove));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackDelete).Subscribe(HandleItemDelete));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObservePlaylistReset).Subscribe(HandleResetPlaylist));

        }
        
        private void UnsubscribeFromServerUpdates()
        {
            foreach (var subscription in _subscriptions) subscription.Dispose();
        }
        
        #endregion Subscriptions

        private ChannelController Controller { get;  }

        public override int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value) return;
                _selectedIndex = value;
                RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => UpdateLoadedStatus(args.Index));
        }

        private void UpdateLoadedStatus(uint index)
        {
            for (var i = 0; i < Tracks.Count; i++) Tracks[i].IsLoaded = i == index;
        }

        protected override bool CanLoadTrack(int track)
        {
            return true;
        }

        protected override void LoadTrack(int track)
        {
            if (track < 0) return;
            Controller?.Select((uint) track);
        }
        
        protected override bool CanResetPlaylist()
        {
            return true;
        }

        protected override void ResetPlaylist()
        {
            Controller?.Reset();
        }

        private bool IsSelectedIndexValid =>
            0 <= SelectedIndex && SelectedIndex < Tracks.Count;

        private bool IsSelectedIndexLoaded =>
            TrackAt(SelectedIndex).IsLoaded;
        
        protected override bool CanDeleteItem()
        {
            return IsSelectedIndexValid && !IsSelectedIndexLoaded;
        }
        
        protected override void DeleteItem()
        {
            if (SelectedIndex < 0) return;
            Controller?.DeleteItemAt((uint)SelectedIndex);
        }

        protected override void DropDirectoryEntry(DirectoryEntry entry)
        {
            Controller?.AddFile(entry);
        }

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(Updates.TrackAddEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Tracks.Add(new TrackViewModel(e.Item)));
        }

        private void HandleItemMove(Updates.TrackMoveEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Tracks.Move((int) e.Index, (int) e.NewIndex));
        }

        private void HandleItemDelete(Updates.TrackDeleteEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Tracks.RemoveAt((int) e.Index));
        }

        private void HandleResetPlaylist(Updates.PlaylistResetEventArgs e)
        {
            // TODO(@MattWindsor91): this should probably _not_ clear the loaded item
            DispatcherHelper.CheckBeginInvokeOnUI(() => Tracks.Clear());
        }

        #endregion Tracklist event handlers
        
        public void Dispose()
        {
            UnsubscribeFromServerUpdates();
        }
    }
}