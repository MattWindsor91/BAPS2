using System;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using URY.BAPS.Client.ViewModel;
using URY.BAPS.Common.Model.Playback;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Abstract base class providing the parts of <see cref="IChannelViewModel" />
    ///     that are largely the same across implementations.
    /// </summary>
    public abstract class ChannelViewModelBase : ChannelComponentViewModelBase, IChannelViewModel
    {
        private RelayCommand? _openAudioWallCommand;
        private RelayCommand<RepeatMode>? _setRepeatModeCommand;
        private RelayCommand? _toggleAutoAdvanceCommand;
        private RelayCommand? _togglePlayOnLoadCommand;

        protected ChannelViewModelBase(ushort channelId,
            IPlayerViewModel? player,
            ITrackListViewModel? trackList) : base(channelId)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            TrackList = trackList ?? throw new ArgumentNullException(nameof(trackList));
        }

        [NotNull]
        public RelayCommand OpenAudioWallCommand => _openAudioWallCommand ??= new RelayCommand(OpenAudioWall, CanOpenAudioWall);

        /// <summary>
        ///     Gets whether the repeat mode is 'none'.
        ///     <para>
        ///         Implementors of <see cref="ChannelViewModelBase" /> should make sure they mark this property as
        ///         changed in their implementation of <see cref="RepeatMode" />.
        ///     </para>
        /// </summary>
        public bool IsRepeatNone => RepeatMode == RepeatMode.None;

        /// <summary>
        ///     Gets whether the repeat mode is 'one'.
        ///     <para>
        ///         Implementors of <see cref="ChannelViewModelBase" /> should make sure they mark this property as
        ///         changed in their implementation of <see cref="RepeatMode" />.
        ///     </para>
        /// </summary>
        public bool IsRepeatOne => RepeatMode == RepeatMode.One;

        /// <summary>
        ///     Gets whether the repeat mode is 'all'.
        ///     <para>
        ///         Implementors of <see cref="ChannelViewModelBase" /> should make sure they mark this property as
        ///         changed in their implementation of <see cref="RepeatMode" />.
        ///     </para>
        /// </summary>
        public bool IsRepeatAll => RepeatMode == RepeatMode.All;

        [NotNull] public IPlayerViewModel Player { get; }
        [NotNull] public ITrackListViewModel TrackList { get; }

        public abstract string Name { get; set; }

        [NotNull]
        public RelayCommand ToggleAutoAdvanceCommand => _toggleAutoAdvanceCommand ??= new RelayCommand(
            () => SetConfigFlag(ChannelFlag.AutoAdvance,
                !IsAutoAdvance),
            () => CanToggleConfig(ChannelFlag.AutoAdvance)
        );

        [NotNull]
        public RelayCommand TogglePlayOnLoadCommand => _togglePlayOnLoadCommand ??= new RelayCommand(
            () => SetConfigFlag(ChannelFlag.PlayOnLoad,
                !IsPlayOnLoad),
            () => CanToggleConfig(ChannelFlag.PlayOnLoad)
        );

        [NotNull]
        public RelayCommand<RepeatMode> SetRepeatModeCommand => _setRepeatModeCommand ??= new RelayCommand<RepeatMode>(
            SetRepeatMode,
            CanSetRepeatMode
        );

        public abstract bool IsPlayOnLoad { get; set; }
        public abstract bool IsAutoAdvance { get; set; }
        public abstract RepeatMode RepeatMode { get; set; }

        /// <summary>
        ///     Checks whether an audio wall can be opened for this channel.
        /// </summary>
        /// <returns>True if <see cref="OpenAudioWallCommand" /> may fire.</returns>
        protected abstract bool CanOpenAudioWall();

        /// <summary>
        ///     Opens an audio wall for this channel.
        /// </summary>
        protected abstract void OpenAudioWall();

        /// <summary>
        ///     Checks whether the repeat mode may be set to the given new mode.
        /// </summary>
        /// <param name="newMode">The proposed new repeat mode.</param>
        /// <returns>True if the repeat mode may be set to <see cref="newMode" />.</returns>
        protected abstract bool CanSetRepeatMode(RepeatMode newMode);

        /// <summary>
        ///     Sets the repeat mode.
        /// </summary>
        /// <param name="newMode">The new repeat mode.</param>
        protected abstract void SetRepeatMode(RepeatMode newMode);

        /// <summary>
        ///     Gets whether a particular channel config setting can be toggled.
        /// </summary>
        /// <param name="setting">The setting to query.</param>
        /// <returns>Whether a toggle action for the given config setting can fire.</returns>
        protected abstract bool CanToggleConfig(ChannelFlag setting);

        /// <summary>
        ///     Implements a change of auto-advance to the given new value.
        /// </summary>
        /// <param name="setting">The setting to query.</param>
        /// <param name="newValue">The intended new value for auto-advance.</param>
        protected abstract void SetConfigFlag(ChannelFlag setting, bool newValue);

        public override void Dispose()
        {
            base.Dispose();

            Player.Dispose();
            TrackList.Dispose();
        }
    }
}