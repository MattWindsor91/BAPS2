using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Common.ServerConfig;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     The view model for a channel.
    ///     <para>
    ///         The channel view model combines a player view model and a track-list.
    ///     </para>
    /// </summary>
    public class ChannelViewModel : ChannelViewModelBase, IDropTarget, IDisposable
    {
        private string _name;

        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();

        private int _selectedIndex = -1;

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
       
        public ChannelViewModel(ushort channelId,
            [CanBeNull] ConfigCache config,
            [CanBeNull] IPlayerViewModel player,
            [CanBeNull] ChannelController controller) : base(channelId, player)
        {
            Controller = controller;

            _config = config;

            RegisterForServerUpdates();
            RegisterForConfigUpdates();
        }

        /// <summary>
        ///     The name of the channel.
        ///     <para>
        ///         If not set manually, this is 'Channel X', where X
        ///         is the channel's ID plus one.
        ///     </para>
        /// </summary>
        public override string Name
        {
            get => _name ?? $"Channel {ChannelId + 1}";
            set
            {
                if (value == _name) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        [CanBeNull] public ChannelController Controller { get; }


        public void Dispose()
        {
            UnregisterForServerUpdates();
            UnregisterForConfigUpdates();
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
                    Controller?.AddFile(dirEntry);
                    break;
            }
        }

        /// <summary>
        ///     Restricts a channel observable to returning only events for this channel.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        [Pure]
        private IObservable<TResult> OnThisChannel<TResult>(IObservable<TResult> source)
            where TResult : ChannelEventArgs
        {
            return from ev in source where ev.ChannelId == ChannelId select ev;
        }
        
        /// <summary>
        ///     Registers the view model against server update events.
        ///     <para>
        ///         This view model can be disposed during run-time, so <see cref="UnregisterForServerUpdates" />
        ///         (called during <see cref="Dispose" />) should be kept in sync with it.
        ///     </para>
        /// </summary>
        private void RegisterForServerUpdates()
        {
            if (Controller == null) return;

            var playbackUpdater = Controller.PlaybackUpdater;
            var playlistUpdater = Controller.PlaylistUpdater;

             // NB: the Player also registers this event.
            _subscriptions.Add(OnThisChannel(playbackUpdater.ObserveTrackLoad).Subscribe(HandleTrackLoad));

            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackAdd).Subscribe(HandleItemAdd));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackMove).Subscribe(HandleItemMove));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObserveTrackDelete).Subscribe(HandleItemDelete));
            _subscriptions.Add(OnThisChannel(playlistUpdater.ObservePlaylistReset).Subscribe(HandleResetPlaylist));

        }

        /// <summary>
        ///     Registers the view model against the config cache's config update events.
        ///     <para>
        ///         This view model can be disposed during run-time, so <see cref="UnregisterForConfigUpdates" />
        ///         (called during <see cref="Dispose" />) should be kept in sync with it.
        ///     </para>
        /// </summary>
        private void RegisterForConfigUpdates()
        {
            _config.ChoiceChanged += HandleConfigChoiceChanged;
            _config.StringChanged += HandleConfigStringChanged;
        }

        /// <summary>
        ///     Unregisters the view model from all server update events.
        /// </summary>
        private void UnregisterForServerUpdates()
        {
            foreach (var subscription in _subscriptions) subscription.Dispose();
        }

        /// <summary>
        ///     Unregisters the view model from all config update events.
        /// </summary>
        private void UnregisterForConfigUpdates()
        {
            _config.ChoiceChanged -= HandleConfigChoiceChanged;
            _config.StringChanged -= HandleConfigStringChanged;
        }

        private void HandleConfigStringChanged(object sender, ConfigCache.StringChangeEventArgs args)
        {
            switch (args.Key)
            {
                case OptionKey.ChannelName:
                    HandleNameChange(args);
                    break;
            }
        }

        private void HandleNameChange(ConfigCache.StringChangeEventArgs args)
        {
            if (ChannelId != args.Index) return;
            Name = args.Value;
        }

        private void HandleConfigChoiceChanged(object sender, ConfigCache.ChoiceChangeEventArgs e)
        {
            switch (e.Key)
            {
                case OptionKey.AutoAdvance:
                    HandleAutoAdvance(e);
                    break;
                case OptionKey.AutoPlay:
                    HandlePlayOnLoad(e);
                    break;
                case OptionKey.Repeat:
                    HandleRepeat(e);
                    break;
            }
        }

        private void HandleAutoAdvance(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            IsAutoAdvance = ChoiceKeys.ChoiceToBoolean(e.Choice, _isAutoAdvance);
        }

        private void HandlePlayOnLoad(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            IsPlayOnLoad = ChoiceKeys.ChoiceToBoolean(e.Choice, _isPlayOnLoad);
        }

        private void HandleRepeat(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            RepeatMode = ChoiceKeys.ChoiceToRepeatMode(e.Choice, _repeatMode);
        }

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => UpdateLoadedStatus(args.Index));
        }

        private void UpdateLoadedStatus(uint index)
        {
            for (var i = 0; i < TrackList.Count; i++) TrackList[i].IsLoaded = i == index;
        }


        protected override bool CanToggleConfig(ChannelFlag setting)
        {
            return true;
        }

        protected override void SetConfigFlag(ChannelFlag configurable, bool value)
        {
            Controller?.SetFlag(configurable, value);
        }

        #region Channel flags

        protected override void DeleteItem()
        {
            if (SelectedIndex < 0) return;
            Controller?.DeleteItemAt((uint)SelectedIndex);
        }

        protected override void SetRepeatMode(RepeatMode newMode)
        {
            Controller?.SetRepeatMode(newMode);
        }

        protected override bool CanSetRepeatMode(RepeatMode newMode)
        {
            return newMode != _repeatMode;
        }

        /// <summary>
        ///     Whether play-on-load is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        public override bool IsPlayOnLoad
        {
            get => _isPlayOnLoad;
            set
            {
                if (_isPlayOnLoad == value) return;
                _isPlayOnLoad = value;
                RaisePropertyChanged(nameof(IsPlayOnLoad));
            }
        }

        private bool _isPlayOnLoad;

        /// <summary>
        ///     Whether auto-advance is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        public override bool IsAutoAdvance
        {
            get => _isAutoAdvance;
            set
            {
                if (_isAutoAdvance == value) return;
                _isAutoAdvance = value;
                RaisePropertyChanged(nameof(IsAutoAdvance));
            }
        }

        private bool _isAutoAdvance;

        public override RepeatMode RepeatMode
        {
            get => _repeatMode;
            set
            {
                if (_repeatMode == value) return;
                _repeatMode = value;
                RaisePropertyChanged(nameof(RepeatMode));
                // Transitive dependencies
                RaisePropertyChanged(nameof(IsRepeatAll));
                RaisePropertyChanged(nameof(IsRepeatOne));
                RaisePropertyChanged(nameof(IsRepeatNone));
            }
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
            0 <= SelectedIndex && SelectedIndex < TrackList.Count;

        private bool IsSelectedIndexLoaded =>
            TrackAt(SelectedIndex).IsLoaded;
        
        protected override bool CanDeleteItem()
        {
            return IsSelectedIndexValid && !IsSelectedIndexLoaded;
        }

        private RepeatMode _repeatMode;
        private readonly ConfigCache _config;

        #endregion Channel flags

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(Updates.TrackAddEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Add(new TrackViewModel(e.Item)));
        }

        private void HandleItemMove(Updates.TrackMoveEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Move((int) e.Index, (int) e.NewIndex));
        }

        private void HandleItemDelete(Updates.TrackDeleteEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.RemoveAt((int) e.Index));
        }

        private void HandleResetPlaylist(Updates.PlaylistResetEventArgs e)
        {
            // TODO(@MattWindsor91): this should probably _not_ clear the loaded item
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Clear());
        }

        #endregion Tracklist event handlers
    }
}