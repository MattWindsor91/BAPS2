using System;
using System.Windows;
using BAPSClientCommon;
using BAPSClientCommon.Events;
using BAPSClientCommon.Model;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     View model governing the player head of a channel.
    /// </summary>
    public class PlayerViewModel : ViewModelBase
    {
        private readonly ushort _id;

        private uint _cuePosition;
        private uint _introPosition;

        /// <summary>
        ///     The currently loaded track.
        /// </summary>
        private ITrack _loadedTrack = new NullTrack();

        private RelayCommand _pauseCommand;

        private RelayCommand _playCommand;
        private uint _position;

        private uint _startTime;
        private ChannelState _state;

        private RelayCommand _stopCommand;

        public PlayerViewModel(ushort id)
        {
            _id = id;
        }

        public PlayerViewModel() : this(0)
        {
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

        /// <summary>
        ///     The currently loaded item (if any).
        /// </summary>
        public ITrack LoadedTrack
        {
            get => _loadedTrack;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_loadedTrack == value) return;

                _loadedTrack = value;
                RaisePropertyChanged(nameof(LoadedTrack));
                RaisePropertyChanged(nameof(Duration));
                // Transitive dependency on Duration
                RaisePropertyChanged(nameof(Remaining));
                RaisePropertyChanged(nameof(PositionScale));
                RaisePropertyChanged(nameof(CuePositionScale));
                RaisePropertyChanged(nameof(IntroPositionScale));
            }
        }

        public ChannelState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                Application.Current.Dispatcher.Invoke(PlayCommand.RaiseCanExecuteChanged);
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
                // Transitive dependency on Position
                RaisePropertyChanged(nameof(PositionScale));
                RaisePropertyChanged(nameof(Remaining));
            }
        }

        /// <summary>
        ///     The position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        public double PositionScale => (double) Position / Duration;

        /// <summary>
        ///     The duration of the currently loaded item (if any), in milliseconds.
        /// </summary>
        public uint Duration => LoadedTrack.Duration;

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
                RaisePropertyChanged(nameof(CuePositionScale));
            }
        }

        /// <summary>
        ///     The cue position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        public double CuePositionScale => (double) CuePosition / Duration;

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
                RaisePropertyChanged(nameof(IntroPositionScale));
            }
        }

        /// <summary>
        ///     The intro position of the currently loaded item (if any),
        ///     as a multiple of the duration.
        /// </summary>
        public double IntroPositionScale => (double) IntroPosition / Duration;

        /// <summary>
        ///     A command that, when fired, asks the server to start playing
        ///     on this channel.
        /// </summary>
        public RelayCommand PlayCommand => _playCommand
                                           ?? (_playCommand = new RelayCommand(
                                               RequestPlay,
                                               () => !IsPlaying));

        /// <summary>
        ///     A command that, when fired, asks the server to pause
        ///     this channel.
        /// </summary>
        public RelayCommand PauseCommand => _pauseCommand
                                            ?? (_pauseCommand = new RelayCommand(
                                                RequestPause));

        private ChannelController Controller =>
            ServiceLocator.Current.GetInstance<ClientCore>().ControllerFor(_id);

        /// <summary>
        ///     A command that, when fired, asks the server to stop
        ///     this channel.
        /// </summary>
        public RelayCommand StopCommand => _stopCommand
                                           ?? (_stopCommand = new RelayCommand(
                                               RequestStop));

        public void Register(IMessenger messenger)
        {
            MessengerInstance = messenger;
            messenger.Register<Updates.PlayerStateEventArgs>(this, HandlePlayerState);
            messenger.Register<Updates.MarkerEventArgs>(this, HandleMarker);
            messenger.Register<Updates.TrackLoadEventArgs>(this, HandleTrackLoad);
        }

        private void HandlePlayerState(Updates.PlayerStateEventArgs id)
        {
            if (id.ChannelId != _id) return;
            State = id.State;
        }

        private void HandleMarker(Updates.MarkerEventArgs args)
        {
            if (args.ChannelId != _id) return;

            switch (args.Marker)
            {
                case MarkerType.Position:
                    Position = args.NewValue;
                    break;
                case MarkerType.Cue:
                    CuePosition = args.NewValue;
                    break;
                case MarkerType.Intro:
                    IntroPosition = args.NewValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RequestPlay()
        {
            Controller.Play();
        }

        private void RequestPause()
        {
            Controller.Pause();
        }

        private void RequestStop()
        {
            Controller.Stop();
        }


        /// <summary>
        ///     Sets all of the channel position markers to zero.
        /// </summary>
        private void Zero()
        {
            Position = 0;
            CuePosition = 0;
            IntroPosition = 0;
        }

        private void HandleTrackLoad(Updates.TrackLoadEventArgs args)
        {
            if (args.ChannelId != _id) return;

            var track = args.Track;
            LoadedTrack = track;
            if (!(track.IsAudioItem || track.IsTextItem)) Zero();
        }
    }
}