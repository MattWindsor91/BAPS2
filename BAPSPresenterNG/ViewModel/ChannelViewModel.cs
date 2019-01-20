using BAPSCommon;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    /// The view model for a channel.
    /// </summary>
    public class ChannelViewModel : ViewModelBase
    {
        public ushort ChannelID { get; }

        public ChannelViewModel(ushort channelID)
        {
            ChannelID = channelID;
        }

        public ChannelController Controller { get; set; }

        /// <summary>
        /// The track list.
        /// </summary>
        public ObservableCollection<EntryInfo> TrackList { get; } = new ObservableCollection<EntryInfo>();

        /// <summary>
        /// Shorthand for accessing the UI thread's dispatcher.
        /// </summary>
        private System.Windows.Threading.Dispatcher UIDispatcher =>
            System.Windows.Application.Current.Dispatcher;

        #region Channel flags

        /// <summary>
        /// Whether this channel is playing, according to the server.
        /// <para>
        /// This property should only be set when the server state
        /// changes.  When the user requests the channel to play, send
        /// <see cref="PlayCommand"/>.
        /// </para>
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;
                UIDispatcher.Invoke(PlayCommand.RaiseCanExecuteChanged);
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }

        private bool _isPlaying = false;

        /// <summary>
        /// Whether this channel is paused, according to the server.
        /// <para>
        /// This property should only be set when the server state
        /// changes.  When the user requests the channel to play, send
        /// <see cref="PauseCommand"/>.
        /// </para>
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value) return;
                _isPaused = value;
                RaisePropertyChanged(nameof(IsPaused));
            }
        }

        private bool _isPaused = false;

        /// <summary>
        /// Whether this channel is stopped, according to the server.
        /// <para>
        /// This property should only be set when the server state
        /// changes.  When the user requests the channel to play, send
        /// <see cref="PauseCommand"/>.
        /// </para>
        /// </summary>
        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                if (_isStopped == value) return;
                _isStopped = value;
                RaisePropertyChanged(nameof(IsStopped));
            }
        }

        private bool _isStopped = false;

        /// <summary>
        /// Whether play-on-load is active, according to the server.
        /// <para>
        /// This property should only be set when the server state
        /// changes.
        /// </para>
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

        private bool _isPlayOnLoad = false;

        /// <summary>
        /// Whether auto-advance is active, according to the server.
        /// <para>
        /// This property should only be set when the server state
        /// changes.
        /// </para>
        /// </summary>
        public bool IsAutoAdvance
        {
            get => _isAutoAdvance;
            set
            {
                if (_isAutoAdvance == value) return;
                _isAutoAdvance = value;
                RaisePropertyChanged(nameof(IsPlayOnLoad));
            }
        }

        private bool _isAutoAdvance = false;

        /// <summary>
        /// Enumeration of repeat modes.
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
        /// A command that, when fired, asks the server to start playing
        /// on this channel.
        /// </summary>
        public RelayCommand PlayCommand => _playCommand
            ?? (_playCommand = new RelayCommand(
                execute: Controller.Play,
                canExecute: () => !IsPlaying));
        private RelayCommand _playCommand;

        /// <summary>
        /// A command that, when fired, asks the server to pause
        /// this channel.
        /// </summary>
        public RelayCommand PauseCommand => _pauseCommand
            ?? (_pauseCommand = new RelayCommand(
                execute: Controller.Pause));
        private RelayCommand _pauseCommand;

        /// <summary>
        /// A command that, when fired, asks the server to stop
        /// this channel.
        /// </summary>
        public RelayCommand StopCommand => _stopCommand
            ?? (_stopCommand = new RelayCommand(
                execute: Controller.Stop));
        private RelayCommand _stopCommand;

        #endregion

        /// <summary>
        /// The duration of the currently loaded item (if any), in milliseconds.
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

        private uint _duration = 0;

        public EntryInfo TrackAt(int index) =>
            TrackList[index] ?? new NullEntryInfo();

        public bool IsLoadPossible(int uindex) =>
            TrackAt(uindex).IsTextItem || !IsPlaying;

        /// <summary>
        /// The position of the currently loaded item (if any), in milliseconds.
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

        private uint _position = 0;

        /// <summary>
        /// The amount of milliseconds remaining in the currently loaded item.
        /// </summary>
        public uint Remaining => Duration - Position;

        /// <summary>
        /// The cue position of the currently loaded item (if any), in milliseconds.
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

        private uint _cuePosition = 0;

        /// <summary>
        /// The intro position of the currently loaded item (if any).
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

        private uint _introPosition = 0;

        /// <summary>
        /// The expected start time of the currently loaded item (if any).
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

        private uint _startTime = 0;

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

        private int _selectedItemIndex = -1;

        /// <summary>
        /// The currently loaded item (if any).
        /// </summary>
        public EntryInfo LoadedTrack
        {
            get => _loadedTrack;
            set
            {
                if (_loadedTrack == value) return;
                _loadedTrack = value;
                RaisePropertyChanged(nameof(LoadedTrack));
            }
        }

        private EntryInfo _loadedTrack = null;

        internal void SetupReactions(Receiver r)
        {
            SetupPlaybackReactions(r);
            SetupPlaylistReactions(r);
            SetupConfigReactions(r);
        }

        private void SetupPlaybackReactions(Receiver r)
        {
            r.ChannelOperation += HandleChannelOperation;
            r.Position += HandlePosition;
            r.Duration += HandleDuration;
            r.LoadedItem += HandleLoadedItem;
            r.TextItem += HandleTextItem;
        }

        private void SetupPlaylistReactions(Receiver r)
        {
            r.ItemAdd += HandleItemAdd;
            r.ItemMove += HandleItemMove;
            r.ItemDelete += HandleItemDelete;
            r.ResetPlaylist += HandleResetPlaylist;
        }

        private void SetupConfigReactions(Receiver r)
        {
            var _config = SimpleIoc.Default.GetInstance<ConfigCache>();
            _config.ConfigChoiceChanged += HandleConfigChoiceChanged;
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
            if (ChannelID != e.Index) return;
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
            if (ChannelID != e.Index) return;
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
            if (ChannelID != e.Index) return;
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

        #region Tracklist event handlers

        // NB: Anything involving the TrackList has to be done on the
        // UI thread, hence the use of Dispatcher.

        private void HandleItemAdd(object sender, (ushort channelID, uint index, EntryInfo entry) e)
        {
            if (ChannelID != e.channelID) return;
            UIDispatcher.Invoke(() => TrackList.Add(e.entry));
        }

        private void HandleItemMove(object sender, (ushort channelID, uint indexFrom, uint indexTo) e)
        {
            if (ChannelID != e.channelID) return;
            UIDispatcher.Invoke(() => TrackList.Move((int)e.indexFrom, (int)e.indexTo));
        }

        private void HandleItemDelete(object sender, (ushort channelID, uint index) e)
        {
            if (ChannelID != e.channelID) return;
            UIDispatcher.Invoke(() => TrackList.RemoveAt((int)e.index));
        }

        private void HandleResetPlaylist(object sender, ushort e)
        {
            if (ChannelID != e) return;
            UIDispatcher.Invoke(() => TrackList.Clear());
        }

        #endregion Tracklist event handlers

        private void HandleDuration(object sender, (ushort channelID, uint duration) e)
        {
            if (ChannelID != e.channelID) return;
            Duration = e.duration;
        }

        private void HandlePosition(object sender, (ushort channelID, PositionType type, uint position) e)
        {
            if (ChannelID != e.channelID) return;
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

        private void HandleChannelOperation(object sender, (ushort channelID, Command op) e)
        {
            if (ChannelID != e.channelID) return;
            IsPlaying = e.op == Command.PLAYBACK;
            IsPaused = e.op == Command.PAUSE;
            IsStopped = e.op == Command.STOP;
        }

        private void HandleTextItem(object sender, (ushort channelID, uint index, TextEntryInfo entry) e)
        {
        }

        private void HandleLoadedItem(object sender, (ushort channelID, uint index, EntryInfo entry) e)
        {
            if (ChannelID != e.channelID) return;

            var entry = e.entry;
            LoadedTrack = entry;
            SelectedItemIndex = (int)e.index;

            if (!entry.IsAudioItem && !entry.IsTextItem)
            {
                Position = 0;
                Duration = 0;
                CuePosition = 0;
                IntroPosition = 0;
            }
        }
    }
}
