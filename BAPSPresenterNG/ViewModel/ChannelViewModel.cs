using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BAPSClientCommon;
using BAPSClientCommon.Controllers;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using BAPSClientCommon.ServerConfig;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     The view model for a channel.
    ///     <para>
    ///         The channel view model combines a player view model and a track-list.
    ///     </para>
    /// </summary>
    public class ChannelViewModel : ViewModelBase, IDropTarget
    {
        [CanBeNull] private RelayCommand _toggleAutoAdvanceCommand;
        [CanBeNull] private RelayCommand _togglePlayOnLoadCommand;

        public ChannelViewModel(ushort channelId, [CanBeNull] IMessenger messenger, [CanBeNull] PlayerViewModel player,
            [CanBeNull] ChannelController controller) : base(messenger)
        {
            ChannelId = channelId;
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Controller = controller;

            Register();
        }

        /// <summary>
        ///     The part of the channel containing the loaded track and its
        ///     position markers.
        /// </summary>
        [NotNull]
        public PlayerViewModel Player { get; }

        /// <summary>
        ///     The name of the channel.
        ///     <para>
        ///         If not set manually, this is 'Channel X', where X
        ///         is the channel's ID plus one.
        ///     </para>
        /// </summary>
        public string Name
        {
            get => _name ?? $"Channel {ChannelId + 1}";
            set
            {
                if (value == _name) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        private string _name;

        private ushort ChannelId { get; }

        [CanBeNull] public ChannelController Controller { get; }

        /// <summary>
        ///     The track list.
        /// </summary>
        [NotNull]
        public ObservableCollection<TrackViewModel> TrackList { get; } = new ObservableCollection<TrackViewModel>();

        /// <summary>
        ///     A command that, when fired, checks the current auto advance
        ///     status and asks the server to invert it.
        /// </summary>
        public RelayCommand ToggleAutoAdvanceCommand => _toggleAutoAdvanceCommand
                                                        ?? (_toggleAutoAdvanceCommand = new RelayCommand(
                                                            () => ToggleConfig(ChannelConfigChangeType.AutoAdvance,
                                                                IsAutoAdvance)));

        /// <summary>
        ///     A command that, when fired, checks the current play-on-load
        ///     status and asks the server to invert it.
        /// </summary>
        public RelayCommand TogglePlayOnLoadCommand => _togglePlayOnLoadCommand
                                                       ?? (_togglePlayOnLoadCommand = new RelayCommand(
                                                           () => ToggleConfig(ChannelConfigChangeType.PlayOnLoad,
                                                               IsPlayOnLoad)));

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

        private TrackViewModel TrackAt(int index)
        {
            return TrackList.ElementAtOrDefault(index) ?? TrackViewModel.MakeNull();
        }

        public bool IsLoadPossible(int index)
        {
            return TrackAt(index).IsTextItem || !Player.IsPlaying;
        }

        private void Register()
        {
            // We use the messenger bus to receive server updates.
            // Assume that the main app attached the server update events to
            // the messenger below.
            var messenger = MessengerInstance;
            Player.Register(messenger);

            // NB: the Player also registers this event.
            messenger.Register<Updates.TrackLoadEventArgs>(this, HandleTrackLoad);

            SetupPlaylistReactions();
            SetupConfigReactions();
        }

        private void SetupPlaylistReactions()
        {
            var messenger = MessengerInstance;
            messenger.Register<Updates.TrackAddEventArgs>(this, HandleItemAdd);
            messenger.Register<Updates.TrackMoveEventArgs>(this, HandleItemMove);
            messenger.Register<Updates.TrackDeleteEventArgs>(this, HandleItemDelete);
            messenger.Register<Updates.ChannelResetEventArgs>(this, HandleResetPlaylist);
        }

        private void SetupConfigReactions()
        {
            var messenger = MessengerInstance;
            messenger.Register<ConfigCache.ChoiceChangeEventArgs>(this, HandleConfigChoiceChanged);
            messenger.Register<ConfigCache.StringChangeEventArgs>(this, HandleConfigStringChanged);
        }

        private void HandleConfigStringChanged(ConfigCache.StringChangeEventArgs args)
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

        private void HandleConfigChoiceChanged(ConfigCache.ChoiceChangeEventArgs e)
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
            switch (e.Choice)
            {
                case ChoiceKeys.RepeatAll:
                    RepeatMode = RepetitionMode.All;
                    break;
                case ChoiceKeys.RepeatNone:
                    RepeatMode = RepetitionMode.None;
                    break;
                case ChoiceKeys.RepeatOne:
                    RepeatMode = RepetitionMode.One;
                    break;
            }
        }

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            if (ChannelId != args.ChannelId) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => UpdateLoadedStatus(args.Index));
        }

        private void UpdateLoadedStatus(uint index)
        {
            for (var i = 0; i < TrackList.Count; i++) TrackList[i].IsLoaded = i == index;
        }


        private void ToggleConfig(ChannelConfigChangeType configurable, bool lastValue)
        {
            var nextValue = lastValue ? ChannelConfigChangeType.Off : ChannelConfigChangeType.On;
            Controller?.Configure(configurable | nextValue);
        }

        #region Channel flags

        /// <summary>
        ///     Whether play-on-load is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        public bool IsPlayOnLoad
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
        public bool IsAutoAdvance
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

        /// <summary>
        ///     Enumeration of repeat modes.
        /// </summary>
        public enum RepetitionMode
        {
            None,
            One,
            All
        }

        public RepetitionMode RepeatMode
        {
            get => _repeatMode;
            set
            {
                if (_repeatMode == value) return;
                _repeatMode = value;
                RaisePropertyChanged(nameof(RepeatMode));
            }
        }

        private RepetitionMode _repeatMode;

        #endregion Channel flags

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(Updates.TrackAddEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Add(new TrackViewModel(e.Item)));
        }

        private void HandleItemMove(Updates.TrackMoveEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Move((int) e.Index, (int) e.NewIndex));
        }

        private void HandleItemDelete(Updates.TrackDeleteEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.RemoveAt((int) e.Index));
        }

        private void HandleResetPlaylist(Updates.ChannelResetEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            DispatcherHelper.CheckBeginInvokeOnUI(() => TrackList.Clear());
        }

        #endregion Tracklist event handlers
    }
}