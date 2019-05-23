using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.ServerConfig;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Client.Wpf.Services;
using URY.BAPS.Common.Model.Playback;
using URY.BAPS.Common.Model.ServerConfig;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     The view model for a channel.
    ///     <para>
    ///         The channel view model combines a player view model and a track-list.
    ///     </para>
    /// </summary>
    public class ChannelViewModel : ChannelViewModelBase
    {
        [CanBeNull] private readonly AudioWallService _audioWallService;

        private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();
        private string _name;

        public ChannelViewModel(ushort channelId,
            [CanBeNull] ConfigCache config,
            [CanBeNull] IPlayerViewModel player,
            [CanBeNull] ITrackListViewModel trackList,
            [CanBeNull] ChannelController controller,
            [CanBeNull] AudioWallService audioWallService) : base(channelId, player, trackList)
        {
            Controller = controller;
            _audioWallService = audioWallService;

            _config = config;

            RegisterForServerUpdates();
            RegisterForConfigUpdates();
        }

        /// <summary>
        ///     The name of the channel.
        ///     <para>
        ///         If not set manually, this is 'Channel X', where X
        ///         is the channel's ID plus one.
        ///     </para>
        /// </summary>
        public override string Name
        {
            get => _name ?? $"Channel {ChannelId + 1}";
            set
            {
                if (value == _name) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        [CanBeNull] public ChannelController Controller { get; }

        public override void Dispose()
        {
            base.Dispose();
            UnsubscribeFromServerUpdates();
            UnsubscribeFromConfigUpdates();
        }

        /// <summary>
        ///     Registers the view model against server update events.
        ///     <para>
        ///         This view model can be disposed during run-time, so <see cref="UnsubscribeFromServerUpdates" />
        ///         (called during <see cref="Dispose" />) should be kept in sync with it.
        ///     </para>
        /// </summary>
        private void RegisterForServerUpdates()
        {
        }

        /// <summary>
        ///     Registers the view model against the config cache's config update events.
        ///     <para>
        ///         This view model can be disposed during run-time, so <see cref="UnsubscribeFromConfigUpdates" />
        ///         (called during <see cref="Dispose" />) should be kept in sync with it.
        ///     </para>
        /// </summary>
        private void RegisterForConfigUpdates()
        {
            _config.ChoiceChanged += HandleConfigChoiceChanged;
            _config.StringChanged += HandleConfigStringChanged;
        }

        /// <summary>
        ///     Unregisters the view model from all server update events.
        /// </summary>
        private void UnsubscribeFromServerUpdates()
        {
            foreach (var subscription in _subscriptions) subscription.Dispose();
        }

        /// <summary>
        ///     Unregisters the view model from all config update events.
        /// </summary>
        private void UnsubscribeFromConfigUpdates()
        {
            _config.ChoiceChanged -= HandleConfigChoiceChanged;
            _config.StringChanged -= HandleConfigStringChanged;
        }

        private void HandleConfigStringChanged(object sender, ConfigCache.StringChangeEventArgs args)
        {
            switch (args.Key)
            {
                case OptionKey.ChannelName:
                    HandleNameChange(args);
                    break;
            }
        }

        private void HandleNameChange(ConfigCache.StringChangeEventArgs args)
        {
            if (ChannelId != args.Index) return;
            Name = args.Value;
        }

        private void HandleConfigChoiceChanged(object sender, ConfigCache.ChoiceChangeEventArgs e)
        {
            switch (e.Key)
            {
                case OptionKey.AutoAdvance:
                    HandleAutoAdvance(e);
                    break;
                case OptionKey.AutoPlay:
                    HandlePlayOnLoad(e);
                    break;
                case OptionKey.Repeat:
                    HandleRepeat(e);
                    break;
            }
        }

        private void HandleAutoAdvance(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            IsAutoAdvance = ChoiceKeys.ChoiceToBoolean(e.Choice, _isAutoAdvance);
        }

        private void HandlePlayOnLoad(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            IsPlayOnLoad = ChoiceKeys.ChoiceToBoolean(e.Choice, _isPlayOnLoad);
        }

        private void HandleRepeat(ConfigCache.ChoiceChangeEventArgs e)
        {
            if (ChannelId != e.Index) return;
            RepeatMode = ChoiceKeys.ChoiceToRepeatMode(e.Choice, _repeatMode);
        }

        protected override bool CanToggleConfig(ChannelFlag setting)
        {
            return true;
        }

        protected override void SetConfigFlag(ChannelFlag configurable, bool value)
        {
            Controller?.SetFlag(configurable, value);
        }

        #region Channel flags

        protected override void SetRepeatMode(RepeatMode newMode)
        {
            Controller?.SetRepeatMode(newMode);
        }

        protected override bool CanOpenAudioWall()
        {
            // TODO(@MattWindsor91): ideally this should reflect whether the
            // AudioWallService already has an audio wall open.
            return _audioWallService != null;
        }

        protected override void OpenAudioWall()
        {
            _audioWallService?.OpenAudioWall(this);
        }

        protected override bool CanSetRepeatMode(RepeatMode newMode)
        {
            return newMode != _repeatMode;
        }

        /// <summary>
        ///     Whether play-on-load is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        public override bool IsPlayOnLoad
        {
            get => _isPlayOnLoad;
            set
            {
                if (_isPlayOnLoad == value) return;
                _isPlayOnLoad = value;
                RaisePropertyChanged(nameof(IsPlayOnLoad));
            }
        }

        private bool _isPlayOnLoad;

        /// <summary>
        ///     Whether auto-advance is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        public override bool IsAutoAdvance
        {
            get => _isAutoAdvance;
            set
            {
                if (_isAutoAdvance == value) return;
                _isAutoAdvance = value;
                RaisePropertyChanged(nameof(IsAutoAdvance));
            }
        }

        private bool _isAutoAdvance;

        public override RepeatMode RepeatMode
        {
            get => _repeatMode;
            set
            {
                if (_repeatMode == value) return;
                _repeatMode = value;
                RaisePropertyChanged(nameof(RepeatMode));
                // Transitive dependencies
                RaisePropertyChanged(nameof(IsRepeatAll));
                RaisePropertyChanged(nameof(IsRepeatOne));
                RaisePropertyChanged(nameof(IsRepeatNone));
            }
        }


        private RepeatMode _repeatMode;
        private readonly ConfigCache _config;

        #endregion Channel flags
    }
}