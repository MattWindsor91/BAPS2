using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public abstract class ChannelViewModelBase : ViewModelBase, IChannelViewModel
    {
        [CanBeNull] private RelayCommand _deleteItemCommand;
        [CanBeNull] private RelayCommand _resetPlaylistCommand;
        [CanBeNull] private RelayCommand<RepeatMode> _setRepeatModeCommand;
        [CanBeNull] private RelayCommand _toggleAutoAdvanceCommand;
        [CanBeNull] private RelayCommand _togglePlayOnLoadCommand;

        protected ChannelViewModelBase(ushort channelId, [CanBeNull] IPlayerViewModel player)
        {
            ChannelId = channelId;
            Player = player ?? throw new ArgumentNullException(nameof(player));
        }

        protected ushort ChannelId { get; }

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

        public abstract string Name { get; set; }

        public abstract int SelectedIndex { get; set; }
            
        [NotNull]
        public ObservableCollection<TrackViewModel> TrackList { get; } = new ObservableCollection<TrackViewModel>();

        [NotNull]
        public RelayCommand ToggleAutoAdvanceCommand => _toggleAutoAdvanceCommand
                                                        ?? (_toggleAutoAdvanceCommand = new RelayCommand(
                                                            () => SetConfigFlag(ChannelFlag.AutoAdvance,
                                                                !IsAutoAdvance),
                                                            () => CanToggleConfig(ChannelFlag.AutoAdvance)
                                                        ));

        [NotNull]
        public RelayCommand TogglePlayOnLoadCommand => _togglePlayOnLoadCommand
                                                       ?? (_togglePlayOnLoadCommand = new RelayCommand(
                                                           () => SetConfigFlag(ChannelFlag.PlayOnLoad,
                                                               !IsPlayOnLoad),
                                                           () => CanToggleConfig(ChannelFlag.PlayOnLoad)
                                                       ));

        [NotNull]
        public RelayCommand<RepeatMode> SetRepeatModeCommand => _setRepeatModeCommand
                                                                ?? (_setRepeatModeCommand =
                                                                    new RelayCommand<RepeatMode>(
                                                                        SetRepeatMode,
                                                                        CanSetRepeatMode
                                                                    ));

        [NotNull]
        public RelayCommand ResetPlaylistCommand => _resetPlaylistCommand
                                                    ?? (_resetPlaylistCommand =
                                                        new RelayCommand(ResetPlaylist, CanResetPlaylist));

        [NotNull]
        public RelayCommand DeleteItemCommand => _deleteItemCommand
                                                 ?? (_deleteItemCommand = new RelayCommand(DeleteItem, CanDeleteItem));

        public abstract bool IsPlayOnLoad { get; set; }
        public abstract bool IsAutoAdvance { get; set; }
        public abstract RepeatMode RepeatMode { get; set; }

        /// <summary>
        ///     Checks whether the reset-playlist command can fire.
        /// </summary>
        /// <returns>Whether <see cref="ResetPlaylistCommand" /> can fire.</returns>
        protected abstract bool CanResetPlaylist();

        /// <summary>
        ///     Resets the channel's playlist.
        /// </summary>
        protected abstract void ResetPlaylist();

        /// <summary>
        ///     Checks whether the delete-item command can fire.
        /// </summary>
        /// <returns>Whether <see cref="DeleteItemCommand" /> can fire.</returns>
        protected abstract bool CanDeleteItem();

        /// <summary>
        ///     Deletes the currently selected item.
        /// </summary>
        protected abstract void DeleteItem();


        /// <summary>
        ///     Sets the repeat mode.
        /// </summary>
        /// <param name="newMode">The new repeat mode.</param>
        protected abstract void SetRepeatMode(RepeatMode newMode);

        /// <summary>
        ///     Checks whether the repeat mode may be set to the given new mode.
        /// </summary>
        /// <param name="newMode">The proposed new repeat mode.</param>
        /// <returns>True if the repeat mode may be set to <see cref="newMode" />.</returns>
        protected abstract bool CanSetRepeatMode(RepeatMode newMode);

        protected TrackViewModel TrackAt(int index)
        {
            return TrackList.ElementAtOrDefault(index) ?? TrackViewModel.MakeNull();
        }

        public bool IsLoadPossible(int index)
        {
            return TrackAt(index).IsTextItem || !Player.IsPlaying;
        }

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
    }
}