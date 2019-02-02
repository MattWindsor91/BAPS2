﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BAPSClientCommon;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;

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
        /// <summary>
        ///     The part of the channel containing the loaded track and its
        ///     position markers.
        /// </summary>
        public PlayerViewModel Player { get; }
       
        private uint _startTime;

        public ChannelViewModel(ushort channelId, PlayerViewModel player, ChannelController controller)
        {
            ChannelId = channelId;
            Player = player;
            Controller = controller;
        }

        public ushort ChannelId { get; }

        public ChannelController Controller { get; }

        /// <summary>
        ///     The track list.
        /// </summary>
        public ObservableCollection<TrackViewModel> TrackList { get; } = new ObservableCollection<TrackViewModel>();

        /// <summary>
        ///     Shorthand for accessing the UI thread's dispatcher.
        /// </summary>
        private static Dispatcher UiDispatcher =>
            Application.Current.Dispatcher;

        /// <summary>
        ///     The expected start time of the currently loaded item (if any).
        /// </summary>
        public uint StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime == value) return;
                _startTime = value;
                RaisePropertyChanged(nameof(StartTime));
            }
        }

        public TrackViewModel TrackAt(int index)
        {
            return TrackList.ElementAtOrDefault(index) ?? new TrackViewModel(new NullTrack()) { IsLoaded = false };
        }

        public bool IsLoadPossible(int index)
        {
            return TrackAt(index).IsTextItem || !Player.IsPlaying;
        }

        internal void Register(IMessenger messenger)
        {
            // We use the messenger bus to receive server updates.
            // Assume that the main app attached the server update events to
            // the messenger below.
            MessengerInstance = messenger;
            Player.Register(messenger);

            // NB: the Player also registers this event.
            messenger.Register<Updates.TrackLoadEventArgs>(this, HandleTrackLoad);
            
            SetupPlaylistReactions(messenger);
            SetupConfigReactions();
        }

        private void SetupPlaylistReactions(IMessenger messenger)
        {
            messenger.Register<Updates.TrackAddEventArgs>(this, HandleItemAdd);
            messenger.Register<Updates.TrackMoveEventArgs>(this, HandleItemMove);
            messenger.Register<Updates.TrackDeleteEventArgs>(this, HandleItemDelete);
            messenger.Register<Updates.ChannelResetEventArgs>(this, HandleResetPlaylist);
        }

        private void SetupConfigReactions()
        {
            var config = SimpleIoc.Default.GetInstance<ConfigCache>();
            config.ConfigChoiceChanged += HandleConfigChoiceChanged;
        }

        private void HandleConfigChoiceChanged(object sender, ConfigChoiceChangeArgs e)
        {
            switch (e.Description)
            {
                case ConfigDescriptions.AutoAdvance:
                    HandleAutoAdvance(e);
                    break;
                case ConfigDescriptions.PlayOnLoad:
                    HandlePlayOnLoad(e);
                    break;
                case ConfigDescriptions.Repeat:
                    HandleRepeat(e);
                    break;
            }
        }

        private void HandleAutoAdvance(ConfigChoiceChangeArgs e)
        {
            if (ChannelId != e.Index) return;
            switch (e.Choice)
            {
                case ChoiceDescriptions.Yes:
                    IsAutoAdvance = true;
                    break;
                case ChoiceDescriptions.No:
                    IsAutoAdvance = false;
                    break;
            }
        }

        private void HandlePlayOnLoad(ConfigChoiceChangeArgs e)
        {
            if (ChannelId != e.Index) return;
            switch (e.Choice)
            {
                case ChoiceDescriptions.Yes:
                    IsPlayOnLoad = true;
                    break;
                case ChoiceDescriptions.No:
                    IsPlayOnLoad = false;
                    break;
            }
        }

        private void HandleRepeat(ConfigChoiceChangeArgs e)
        {
            if (ChannelId != e.Index) return;
            switch (e.Choice)
            {
                case ChoiceDescriptions.RepeatAll:
                    RepeatMode = RepetitionMode.All;
                    break;
                case ChoiceDescriptions.RepeatNone:
                    RepeatMode = RepetitionMode.None;
                    break;
                case ChoiceDescriptions.RepeatOne:
                    RepeatMode = RepetitionMode.One;
                    break;
            }
        }

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            if (ChannelId != args.ChannelId) return;
            UiDispatcher.Invoke(() => UpdateLoadedStatus(args.Index));
        }

        private void UpdateLoadedStatus(uint index)
        {
            for (var i = 0; i < TrackList.Count; i++)
            {
                TrackList[i].IsLoaded = i == index;
            }
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


        /// <summary>
        ///     A command that, when fired, checks the current auto advance
        ///     status and asks the server to invert it.
        /// </summary>
        public RelayCommand ToggleAutoAdvanceCommand => _toggleAutoAdvanceCommand
                                                        ?? (_toggleAutoAdvanceCommand = new RelayCommand(
                                                            () => ToggleConfig(ChannelConfigChangeType.AutoAdvance,
                                                                IsAutoAdvance)));

        private RelayCommand _toggleAutoAdvanceCommand;

        /// <summary>
        ///     A command that, when fired, checks the current play-on-load
        ///     status and asks the server to invert it.
        /// </summary>
        public RelayCommand TogglePlayOnLoadCommand => _togglePlayOnLoadCommand
                                                       ?? (_togglePlayOnLoadCommand = new RelayCommand(
                                                           () => ToggleConfig(ChannelConfigChangeType.PlayOnLoad,
                                                               IsPlayOnLoad)));

        private RelayCommand _togglePlayOnLoadCommand;


        private void ToggleConfig(ChannelConfigChangeType configurable, bool lastValue)
        {
            var nextValue = lastValue ? ChannelConfigChangeType.Off : ChannelConfigChangeType.On;
            Controller.Configure(configurable | nextValue);
        }

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(Updates.TrackAddEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.Add(new TrackViewModel(e.Item)));
        }

        private void HandleItemMove(Updates.TrackMoveEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.Move((int) e.Index, (int) e.NewIndex));
        }

        private void HandleItemDelete(Updates.TrackDeleteEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.RemoveAt((int) e.Index));
        }

        private void HandleResetPlaylist(Updates.ChannelResetEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.Clear());
        }

        #endregion Tracklist event handlers

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
                    Controller.AddFile(dirEntry);
                    break;
            }
        }
    }
}