using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BAPSClientCommon;
using BAPSClientCommon.ServerConfig;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    ///     The view model for the main BAPS Presenter window.
    ///     <para>
    ///         This view model collects together everything needed to display the main window:
    ///         channels, directories, and text.
    ///     </para>
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand<ushort> _forwardPauseCommand;

        private RelayCommand<ushort> _forwardPlayCommand;

        private RelayCommand<ushort> _forwardStopCommand;
        private string _text;

        public MainViewModel(IMessenger messenger) : base(messenger)
        {
            Text = "<You can type notes here>";
            Register();
        }

        /// <summary>
        ///     The set of channels currently in use.
        /// </summary>
        public ObservableCollection<ChannelViewModel> Channels { get; } = new ObservableCollection<ChannelViewModel>();

        public ObservableCollection<DirectoryViewModel> Directories { get; } =
            new ObservableCollection<DirectoryViewModel>();

        /// <summary>
        ///     A command that, when executed, sends a play command to the channel
        ///     with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardPlayCommand =>
            _forwardPlayCommand
            ?? (_forwardPlayCommand = new RelayCommand<ushort>(
                channelID => { ChannelAt(channelID)?.Player?.PlayCommand?.Execute(null); },
                channelID =>
                    ChannelAt(channelID)?.Player?.PlayCommand?.CanExecute(null) ?? false
            ));

        /// <summary>
        ///     A command that, when executed, sends a pause command to the channel
        ///     with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardPauseCommand =>
            _forwardPauseCommand
            ?? (_forwardPauseCommand = new RelayCommand<ushort>(
                channelID => { ChannelAt(channelID)?.Player?.PauseCommand?.Execute(null); },
                channelID =>
                    ChannelAt(channelID)?.Player?.PauseCommand?.CanExecute(null) ?? false
            ));

        /// <summary>
        ///     A command that, when executed, sends a stop command to the channel
        ///     with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardStopCommand =>
            _forwardStopCommand
            ?? (_forwardStopCommand = new RelayCommand<ushort>(
                channelID => { ChannelAt(channelID)?.Player?.StopCommand?.Execute(null); },
                channelID =>
                    ChannelAt(channelID)?.Player?.StopCommand?.CanExecute(null) ?? false
            ));

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        /// <summary>
        ///     Shorthand for getting the channel at the given channel ID.
        /// </summary>
        /// <param name="channelID">The ID of the channel to get.</param>
        /// <returns>The channel at <paramref name="channelID" />, or null if one doesn't exist.</returns>
        public ChannelViewModel ChannelAt(ushort channelID)
        {
            return Channels?.ElementAtOrDefault(channelID);
        }

        private void Register()
        {
            var messenger = MessengerInstance;
            messenger.Register<Cache.IntChangeEventArgs>(this, HandleConfigIntChange);
        }

        private void HandleConfigIntChange(Cache.IntChangeEventArgs args)
        {
            switch (args.Key)
            {
                case SettingKey.ChannelCount:
                    HandleChannelCountChange(args.Value);
                    break;
                case SettingKey.DirectoryCount:
                    HandleDirectoryCountChange(args.Value);
                    break;
            }
        }

        private void UpdateObservable<T>(IEnumerable<T> objects, ICollection<T> target)
        {
            target.Clear();
            foreach (var o in objects) target.Add(o);
        }

        private void HandleCountChange<T>(int newCount, ICollection<T> observableTarget, Func<ushort, T> factory)
        {
            if (newCount == observableTarget.Count) return;
            var newObjects = new T[newCount];
            for (ushort i = 0; i < newCount; i++) newObjects[i] = factory(i);
            DispatcherHelper.CheckBeginInvokeOnUI(() => UpdateObservable(newObjects, observableTarget));
        }

        private void HandleChannelCountChange(int newChannelCount)
        {
            HandleCountChange(newChannelCount, Channels, MakeChannelViewModel);
        }

        private void HandleDirectoryCountChange(int newDirectoryCount)
        {
            HandleCountChange(newDirectoryCount, Directories, MakeDirectoryViewModel);
        }

        private ChannelViewModel MakeChannelViewModel(ushort channelId)
        {
            var player = new PlayerViewModel(channelId);
            var core = ServiceLocator.Current.GetInstance<ClientCore>();
            var controller = core.ControllerFor(channelId);
            return new ChannelViewModel(channelId, MessengerInstance, player, controller);
        }

        private DirectoryViewModel MakeDirectoryViewModel(ushort directoryId)
        {
            return new DirectoryViewModel(directoryId, MessengerInstance);
        }
    }
}