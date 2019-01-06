using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;
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
