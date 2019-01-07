using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;

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
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Text = "<You can type notes here>";
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
                          ChannelAt(channelID)?.PlayCommand?.Execute(null);
                      },
                      canExecute: channelID =>
                        ChannelAt(channelID)?.PlayCommand?.CanExecute(null) ?? false
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
                          ChannelAt(channelID)?.PauseCommand?.Execute(null);
                      },
                      canExecute: channelID =>
                        ChannelAt(channelID)?.PauseCommand?.CanExecute(null) ?? false
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
                          ChannelAt(channelID)?.StopCommand?.Execute(null);
                      },
                      canExecute: channelID =>
                        ChannelAt(channelID)?.StopCommand?.CanExecute(null) ?? false
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