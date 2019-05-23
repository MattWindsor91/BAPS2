using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Wpf.Services;
using URY.BAPS.Common.Model.ServerConfig;

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
        [NotNull] private readonly ChannelFactoryService _channelFactory;
        [NotNull] private readonly ConfigCache _config;
        [NotNull] private readonly DirectoryFactoryService _directoryFactory;

        [CanBeNull] private RelayCommand<ushort> _forwardPauseCommand;

        [CanBeNull] private RelayCommand<ushort> _forwardPlayCommand;

        [CanBeNull] private RelayCommand<ushort> _forwardStopCommand;

        [NotNull] public ITextViewModel Text { get; }

        public MainViewModel(
            [CanBeNull] ChannelFactoryService channelFactory,
            [CanBeNull] DirectoryFactoryService directoryFactory,
            [CanBeNull] ConfigCache config,
            [CanBeNull] ITextViewModel text)
        {
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
            _directoryFactory = directoryFactory ?? throw new ArgumentNullException(nameof(directoryFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            RegisterForConfigUpdates();
        }

        /// <summary>
        ///     The set of channels currently in use.
        /// </summary>
        [NotNull]
        public ObservableCollection<IChannelViewModel> Channels { get; } = new ObservableCollection<IChannelViewModel>();

        [NotNull]
        public ObservableCollection<IDirectoryViewModel> Directories { get; } =
            new ObservableCollection<IDirectoryViewModel>();

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

        /// <summary>
        ///     Shorthand for getting the channel at the given channel ID.
        /// </summary>
        /// <param name="channelId">The ID of the channel to get.</param>
        /// <returns>The channel at <paramref name="channelId" />, or null if one doesn't exist.</returns>
        [CanBeNull]
        private IChannelViewModel ChannelAt(ushort channelId)
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

        private static void HandleCountChange<T>(int newCount, ICollection<T> observableTarget, Func<byte, T> factory)
            where T : IDisposable
        {
            if (newCount == observableTarget.Count) return;
            var newObjects = new T[newCount];
            for (byte i = 0; i < newCount; i++) newObjects[i] = factory(i);
            DispatcherHelper.CheckBeginInvokeOnUI(() => UpdateObservable(newObjects, observableTarget));
        }

        private void HandleChannelCountChange(int newChannelCount)
        {
            HandleCountChange(newChannelCount, Channels, _channelFactory.Make);
        }

        private void HandleDirectoryCountChange(int newDirectoryCount)
        {
            HandleCountChange(newDirectoryCount, Directories, _directoryFactory.Make);
        }
    }
}