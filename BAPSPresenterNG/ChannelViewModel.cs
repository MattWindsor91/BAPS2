using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BAPSCommon;

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

        /// <summary>
        /// The name of the currently loaded item (if any).
        /// </summary>
        public string LoadedTrackName
        {
            get => _loadedTrackName;
            set
            {
                if (_loadedTrackName == value) return;
                _loadedTrackName = value;
                OnPropertyChanged(nameof(LoadedTrackName));
            }
        }
        private string _loadedTrackName = "NONE";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void SetupPlaybackReactions(Receiver r)
        {
            var myID = ChannelID;

            r.ChannelOperation += HandleChannelOperation;
            r.LoadedItem += HandleLoadedItem;
            r.TextItem += HandleTextItem;
            r.Position += HandlePosition;
            r.Duration += HandleDuration;
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

        private void HandleTextItem(object sender, (ushort channelID, uint index, string description, string text) e)
        {
        }

        private void HandleLoadedItem(object sender, (ushort channelID, uint index, Command type, string description) e)
        {
            if (ChannelID != e.channelID) return;
            LoadedTrackName = e.description;
        }
    }
}
