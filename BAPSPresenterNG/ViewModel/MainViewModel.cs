using BAPSClientCommon;
using BAPSClientCommon.Events;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Threading;

namespace BAPSPresenterNG.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IMessenger messenger) : base(messenger)
        {
            Text = "<You can type notes here>";
            Register();
        }

        /// <summary>
        /// The set of channels currently in use.
        /// </summary>
        public ObservableCollection<ChannelViewModel> Channels { get; } = new ObservableCollection<ChannelViewModel>();

        /// <summary>
        /// Shorthand for getting the channel at the given channel ID.
        /// </summary>
        /// <param name="channelID">The ID of the channel to get.</param>
        /// <returns>The channel at <paramref name="channelID"/>, or null if one doesn't exist.</returns>
        public ChannelViewModel ChannelAt(ushort channelID) => Channels?.ElementAtOrDefault(channelID);

        public ObservableCollection<DirectoryViewModel> Directories { get; } = new ObservableCollection<DirectoryViewModel>();

        private void Register()
        {
            var messenger = MessengerInstance;
            messenger.Register(this, (Action<Updates.DirectoryPrepareArgs>)HandleDirectoryPrepare);
        }

        /// <summary>
        /// Handles a directory-prepare server update.
        /// <para>
        /// This view model only handles directory-prepares that don't
        /// correspond to existing directory view models; the corresponding 
        /// view models themselves handle those updates.
        /// </para>
        /// </summary>
        /// <param name="e"></param>
        private void HandleDirectoryPrepare(Updates.DirectoryPrepareArgs e)
        {
            var dir = new DirectoryViewModel(MessengerInstance, e.DirectoryId)
            {
                Name = e.Name
            };
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                // Assume that, since this is the only thing that inserts into
                // the directories list, the list is always sorted by
                // directory ID.
                var above = Directories.TakeWhile(x => x.DirectoryId <= e.DirectoryId);
                if (above.LastOrDefault()?.DirectoryId == e.DirectoryId) return; // Already present.
                var i = above.Count();
                Directories.Insert(i, dir);
            });
        }

        private RelayCommand<ushort> _forwardPlayCommand = null;

        /// <summary>
        /// A command that, when executed, sends a play command to the channel
        /// with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardPlayCommand =>
            _forwardPlayCommand
                  ?? (_forwardPlayCommand = new RelayCommand<ushort>(
                      execute: channelID =>
                      {
                          ChannelAt(channelID)?.Player?.PlayCommand?.Execute(null);
                      },
                      canExecute: channelID =>
                        ChannelAt(channelID)?.Player?.PlayCommand?.CanExecute(null) ?? false
                      ));

        private RelayCommand<ushort> _forwardPauseCommand = null;

        /// <summary>
        /// A command that, when executed, sends a pause command to the channel
        /// with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardPauseCommand =>
            _forwardPauseCommand
                  ?? (_forwardPauseCommand = new RelayCommand<ushort>(
                      execute: channelID =>
                      {
                          ChannelAt(channelID)?.Player?.PauseCommand?.Execute(null);
                      },
                      canExecute: channelID =>
                        ChannelAt(channelID)?.Player?.PauseCommand?.CanExecute(null) ?? false
                      ));

        private RelayCommand<ushort> _forwardStopCommand = null;

        /// <summary>
        /// A command that, when executed, sends a stop command to the channel
        /// with the given index.
        /// </summary>
        public RelayCommand<ushort> ForwardStopCommand =>
            _forwardStopCommand
                  ?? (_forwardStopCommand = new RelayCommand<ushort>(
                      execute: channelID =>
                      {
                          ChannelAt(channelID)?.Player?.StopCommand?.Execute(null);
                      },
                      canExecute: channelID =>
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
        private string _text;
    }
}