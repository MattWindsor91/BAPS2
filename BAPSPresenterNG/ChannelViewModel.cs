using BAPSCommon;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace BAPSPresenterNG
{
    /// <summary>
    /// The view model for a channel.
    /// </summary>
    public class ChannelViewModel : INotifyPropertyChanged
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

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        private bool _isPlaying = false;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value) return;
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }
        private bool _isPaused = false;

        public bool IsStopped
        {
            get => _isStopped;
            set
            {
                if (_isStopped == value) return;
                _isStopped = value;
                OnPropertyChanged(nameof(IsStopped));
            }
        }
        private bool _isStopped = false;

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
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(Remaining));
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
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Remaining));
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
                OnPropertyChanged(nameof(CuePosition));
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
                OnPropertyChanged(nameof(IntroPosition));
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
                OnPropertyChanged(nameof(StartTime));
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
                OnPropertyChanged(nameof(SelectedItemIndex));
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
                OnPropertyChanged(nameof(LoadedTrack));
            }
        }
        private EntryInfo _loadedTrack = null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void SetupPlaybackReactions(Receiver r)
        {
            r.ChannelOperation += HandleChannelOperation;
            r.Position += HandlePosition;
            r.Duration += HandleDuration;
            r.LoadedItem += HandleLoadedItem;
            r.TextItem += HandleTextItem;
        }

        internal void SetupPlaylistReactions(Receiver r)
        {
            r.ItemAdd += HandleItemAdd;
            r.ItemMove += HandleItemMove;
            r.ItemDelete += HandleItemDelete;
            r.ResetPlaylist += HandleResetPlaylist;
        }

        private void HandleItemAdd(object sender, (ushort channelID, uint index, EntryInfo entry) e)
        {
            if (ChannelID != e.channelID) return;
            TrackList.Add(e.entry);
        }

        private void HandleItemMove(object sender, (ushort channelID, uint indexFrom, uint indexTo) e)
        {
            if (ChannelID != e.channelID) return;
            TrackList.Move((int)e.indexFrom, (int)e.indexTo);
        }

        private void HandleItemDelete(object sender, (ushort channelID, uint index) e)
        {
            if (ChannelID != e.channelID) return;
            TrackList.RemoveAt((int)e.index);
        }

        private void HandleResetPlaylist(object sender, ushort e)
        {
            if (ChannelID != e) return;
            TrackList.Clear();
        }

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
