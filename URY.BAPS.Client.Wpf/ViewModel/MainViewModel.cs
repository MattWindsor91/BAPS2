using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Common.Updaters;
using URY.BAPS.Client.Wpf.Services;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     The view model for the main BAPS Presenter window.
    ///     <para>
    ///         This view model collects together everything needed to display the main window:
    ///         channels, directories, and text.
    ///     </para>
    /// </summary>
    [UsedImplicitly]
    public class MainViewModel : ViewModelBase
    {
        [NotNull] private readonly ConfigCache _config;
        [NotNull] private readonly ChannelControllerSet _channelControllerSet;
        [NotNull] private readonly DirectoryControllerSet _directoryControllerSet;
        [NotNull] private readonly AudioWallService _audioWallService;

        [NotNull] private readonly IServerUpdater _updater;


        [CanBeNull] private RelayCommand<ushort> _forwardPauseCommand;

        [CanBeNull] private RelayCommand<ushort> _forwardPlayCommand;

        [CanBeNull] private RelayCommand<ushort> _forwardStopCommand;
        private string _text;

        public MainViewModel(
            [CanBeNull] ConfigCache config,
            // TODO(@MattWindsor91): this should be IServerUpdater, but I'm not sure how to get SimpleIoC to inject it otherwise.
            [CanBeNull] IClientCore updater,
            [CanBeNull] ChannelControllerSet controllerSet,
            [CanBeNull] DirectoryControllerSet directoryControllerSet,
            [CanBeNull] AudioWallService audioWallService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _channelControllerSet = controllerSet ?? throw new ArgumentNullException(nameof(controllerSet));
            _directoryControllerSet = directoryControllerSet ?? throw new ArgumentNullException(nameof(directoryControllerSet));
            _audioWallService = audioWallService ?? throw new ArgumentNullException(nameof(audioWallService));
            _updater = updater ?? throw new ArgumentNullException(nameof(updater));

            Text = "<You can type notes here>";
            RegisterForConfigUpdates();
        }

        /// <summary>
        ///     The set of channels currently in use.
        /// </summary>
        [NotNull]
        public ObservableCollection<ChannelViewModel> Channels { get; } = new ObservableCollection<ChannelViewModel>();

        [NotNull]
        public ObservableCollection<DirectoryViewModel> Directories { get; } =
            new ObservableCollection<DirectoryViewModel>();

        /// <summary>
        ///     A command that, when executed, sends a play command to the channel
        ///     with the given index.
        /// </summary>
        [NotNull]
        public RelayCommand<ushort> ForwardPlayCommand =>
            _forwardPlayCommand
            ?? (_forwardPlayCommand = new RelayCommand<ushort>(
                channelId => { ChannelAt(channelId)?.Player?.PlayCommand?.Execute(null); },
                channelId =>
                    ChannelAt(channelId)?.Player?.PlayCommand?.CanExecute(null) ?? false
            ));

        /// <summary>
        ///     A command that, when executed, sends a pause command to the channel
        ///     with the given index.
        /// </summary>
        [NotNull]
        public RelayCommand<ushort> ForwardPauseCommand =>
            _forwardPauseCommand
            ?? (_forwardPauseCommand = new RelayCommand<ushort>(
                channelId => { ChannelAt(channelId)?.Player?.PauseCommand?.Execute(null); },
                channelId =>
                    ChannelAt(channelId)?.Player?.PauseCommand?.CanExecute(null) ?? false
            ));

        /// <summary>
        ///     A command that, when executed, sends a stop command to the channel
        ///     with the given index.
        /// </summary>
        [NotNull]
        public RelayCommand<ushort> ForwardStopCommand =>
            _forwardStopCommand
            ?? (_forwardStopCommand = new RelayCommand<ushort>(
                channelId => { ChannelAt(channelId)?.Player?.StopCommand?.Execute(null); },
                channelId =>
                    ChannelAt(channelId)?.Player?.StopCommand?.CanExecute(null) ?? false
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
        /// <param name="channelId">The ID of the channel to get.</param>
        /// <returns>The channel at <paramref name="channelId" />, or null if one doesn't exist.</returns>
        [CanBeNull]
        private ChannelViewModel ChannelAt(ushort channelId)
        {
            return Channels.ElementAtOrDefault(channelId);
        }

        /// <summary>
        ///     Registers the view model against the config cache's config update events.
        ///     <para>
        ///         This view model lasts until the presenter quits, so we don't manually unregister from these events.
        ///     </para>
        /// </summary>
        private void RegisterForConfigUpdates()
        {
            _config.IntChanged += HandleConfigIntChange;
        }

        private void HandleConfigIntChange(object sender, ConfigCache.IntChangeEventArgs args)
        {
            switch (args.Key)
            {
                case OptionKey.ChannelCount:
                    HandleChannelCountChange(args.Value);
                    break;
                case OptionKey.DirectoryCount:
                    HandleDirectoryCountChange(args.Value);
                    break;
            }
        }

        private static void UpdateObservable<T>(IEnumerable<T> objects, ICollection<T> target)
            where T : IDisposable
        {
            foreach (var o in target) o.Dispose();
            target.Clear();
            foreach (var o in objects) target.Add(o);
        }

        private static void HandleCountChange<T>(int newCount, ICollection<T> observableTarget, Func<ushort, T> factory)
            where T : IDisposable
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

        [Pure]
        private ChannelViewModel MakeChannelViewModel(ushort channelId)
        {
            var controller = _channelControllerSet.ControllerFor(channelId);
            var player = new PlayerViewModel(channelId, controller);
            var trackList = new TrackListViewModel(channelId, controller);
            return new ChannelViewModel(channelId, _config, player, trackList, controller, _audioWallService);
        }

        [Pure]
        private DirectoryViewModel MakeDirectoryViewModel(ushort directoryId)
        {
            var controller = _directoryControllerSet.ControllerFor(directoryId);
            return new DirectoryViewModel(directoryId, controller);
        }
    }
}