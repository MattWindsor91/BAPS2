using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using BAPSClientCommon;
using BAPSClientCommon.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     The view model for a channel.
    /// </summary>
    public class ChannelViewModel : ViewModelBase, IDropTarget
    {
        private uint _cuePosition;

        private uint _duration;

        private uint _introPosition;

        private Track _loadedTrack;

        private uint _position;

        private int _selectedItemIndex = -1;

        private uint _startTime;

        public ChannelViewModel(ushort channelId)
        {
            ChannelId = channelId;
        }

        public ushort ChannelId { get; }

        public ChannelController Controller { get; set; }

        /// <summary>
        ///     The track list.
        /// </summary>
        public ObservableCollection<Track> TrackList { get; } = new ObservableCollection<Track>();

        /// <summary>
        ///     Shorthand for accessing the UI thread's dispatcher.
        /// </summary>
        private static Dispatcher UiDispatcher =>
            Application.Current.Dispatcher;

        /// <summary>
        ///     The duration of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public uint Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                RaisePropertyChanged(nameof(Duration));
                RaisePropertyChanged(nameof(Remaining));
            }
        }

        /// <summary>
        ///     The position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public uint Position
        {
            get => _position;
            set
            {
                if (_position == value) return;
                _position = value;
                RaisePropertyChanged(nameof(Position));
                RaisePropertyChanged(nameof(Remaining));
            }
        }

        /// <summary>
        ///     The amount of milliseconds remaining in the currently loaded item.
        /// </summary>
        public uint Remaining => Duration - Position;

        /// <summary>
        ///     The cue position of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public uint CuePosition
        {
            get => _cuePosition;
            set
            {
                if (_cuePosition == value) return;
                _cuePosition = value;
                RaisePropertyChanged(nameof(CuePosition));
            }
        }

        /// <summary>
        ///     The intro position of the currently loaded item (if any).
        /// </summary>
        public uint IntroPosition
        {
            get => _introPosition;
            set
            {
                if (_introPosition == value) return;
                _introPosition = value;
                RaisePropertyChanged(nameof(IntroPosition));
            }
        }

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

        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (_selectedItemIndex == value) return;
                _selectedItemIndex = value;
                RaisePropertyChanged(nameof(SelectedItemIndex));
            }
        }

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        public Track LoadedTrack
        {
            get => _loadedTrack;
            set
            {
                if (_loadedTrack == value) return;
                _loadedTrack = value;
                RaisePropertyChanged(nameof(LoadedTrack));
            }
        }

        public Track TrackAt(int index)
        {
            return TrackList[index] ?? new NullTrack();
        }

        public bool IsLoadPossible(int index)
        {
            return TrackAt(index).IsTextItem || !IsPlaying;
        }

        internal void SetupReactions(Receiver r)
        {
            // We use the messenger bus to receive server updates.
            // Assume that the main app attached the server update events to
            // the messenger below.
            var messenger = MessengerInstance ?? Messenger.Default;

            SetupPlaybackReactions(r, messenger);
            SetupPlaylistReactions(messenger);
            SetupConfigReactions();
        }

        private void SetupPlaybackReactions(IServerUpdater r, IMessenger messenger)
        {
            messenger.Register(this, (Action<ServerUpdates.ChannelStateEventArgs>) HandleChannelState);

            r.Position += HandlePosition;
            r.Duration += HandleDuration;
            r.LoadedItem += HandleLoadedItem;
            r.TextItem += HandleTextItem;
        }

        private void SetupPlaylistReactions(IMessenger messenger)
        {
            messenger.Register(this, (Action<ServerUpdates.ItemAddEventArgs>) HandleItemAdd);
            messenger.Register(this, (Action<ServerUpdates.ItemMoveEventArgs>) HandleItemMove);
            messenger.Register(this, (Action<ServerUpdates.ItemDeleteEventArgs>) HandleItemDelete);
            messenger.Register(this, (Action<ServerUpdates.ChannelResetEventArgs>) HandleResetPlaylist);
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

        private void HandleDuration(object sender, (ushort channelID, uint duration) e)
        {
            if (ChannelId != e.channelID) return;
            Duration = e.duration;
        }

        private void HandlePosition(object sender, (ushort channelID, PositionType type, uint position) e)
        {
            if (ChannelId != e.channelID) return;
            switch (e.type)
            {
                case PositionType.Position:
                    Position = e.position;
                    break;

                case PositionType.Cue:
                    CuePosition = e.position;
                    break;

                case PositionType.Intro:
                    IntroPosition = e.position;
                    break;
            }
        }

        private void HandleChannelState(ServerUpdates.ChannelStateEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            State = e.State;
        }

        private void HandleTextItem(object sender, (ushort channelID, uint index, TextTrack entry) e)
        {
        }

        private void HandleLoadedItem(object sender, (ushort channelID, uint index, Track entry) e)
        {
            if (ChannelId != e.channelID) return;

            var entry = e.entry;
            LoadedTrack = entry;
            SelectedItemIndex = (int) e.index;

            if (!entry.IsAudioItem && !entry.IsTextItem)
            {
                Position = 0;
                Duration = 0;
                CuePosition = 0;
                IntroPosition = 0;
            }
        }

        #region Channel flags

        private ChannelState _state;

        public ChannelState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                UiDispatcher.Invoke(PlayCommand.RaiseCanExecuteChanged);
                RaisePropertyChanged(nameof(State));
                // Derived properties
                RaisePropertyChanged(nameof(IsPlaying));
                RaisePropertyChanged(nameof(IsPaused));
                RaisePropertyChanged(nameof(IsStopped));
            }
        }

        /// <summary>
        ///     Whether this channel is playing, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to play, send
        ///         <see cref="PlayCommand" />.
        ///     </para>
        /// </summary>
        public bool IsPlaying => _state == ChannelState.Playing;

        /// <summary>
        ///     Whether this channel is paused, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to play, send
        ///         <see cref="PauseCommand" />.
        ///     </para>
        /// </summary>
        public bool IsPaused => _state == ChannelState.Paused;

        /// <summary>
        ///     Whether this channel is stopped, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.  When the user requests the channel to play, send
        ///         <see cref="PauseCommand" />.
        ///     </para>
        /// </summary>
        public bool IsStopped => _state == ChannelState.Stopped;

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

        #region Commands

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        public RelayCommand PlayCommand => _playCommand
                                           ?? (_playCommand = new RelayCommand(
                                               Controller.Play,
                                               () => !IsPlaying));

        private RelayCommand _playCommand;

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        public RelayCommand PauseCommand => _pauseCommand
                                            ?? (_pauseCommand = new RelayCommand(
                                                Controller.Pause));

        private RelayCommand _pauseCommand;

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        public RelayCommand StopCommand => _stopCommand
                                           ?? (_stopCommand = new RelayCommand(
                                               Controller.Stop));

        private RelayCommand _stopCommand;

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

        #endregion

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(ServerUpdates.ItemAddEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.Add(e.Item));
        }

        private void HandleItemMove(ServerUpdates.ItemMoveEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.Move((int) e.Index, (int) e.NewIndex));
        }

        private void HandleItemDelete(ServerUpdates.ItemDeleteEventArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            UiDispatcher.Invoke(() => TrackList.RemoveAt((int) e.Index));
        }

        private void HandleResetPlaylist(ServerUpdates.ChannelResetEventArgs e)
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