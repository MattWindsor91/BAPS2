using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using URY.BAPS.Client.Common;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public abstract class ChannelViewModelBase : ViewModelBase, IChannelViewModel
    {
        [CanBeNull] private RelayCommand _toggleAutoAdvanceCommand;
        [CanBeNull] private RelayCommand _togglePlayOnLoadCommand;

        protected ChannelViewModelBase(ushort channelId, [CanBeNull] IPlayerViewModel player)
        {
            ChannelId = channelId;
            Player = player ?? throw new ArgumentNullException(nameof(player));
        }

        protected ushort ChannelId { get; }

        [NotNull] public IPlayerViewModel Player { get; }

        public abstract string Name { get; set; }

        /// <summary>
        ///     The track list.
        /// </summary>
        [NotNull]
        public ObservableCollection<TrackViewModel> TrackList { get; } = new ObservableCollection<TrackViewModel>();

        /// <summary>
        ///     A command that, when fired, checks the current auto advance
        ///     status and asks the server to invert it.
        /// </summary>
        [NotNull]
        public RelayCommand ToggleAutoAdvanceCommand => _toggleAutoAdvanceCommand
                                                        ?? (_toggleAutoAdvanceCommand = new RelayCommand(
                                                            () => SetConfigFlag(ChannelConfigChangeType.AutoAdvance,
                                                                !IsAutoAdvance)));

        /// <summary>
        ///     A command that, when fired, checks the current play-on-load
        ///     status and asks the server to invert it.
        /// </summary>
        [NotNull]
        public RelayCommand TogglePlayOnLoadCommand => _togglePlayOnLoadCommand
                                                       ?? (_togglePlayOnLoadCommand = new RelayCommand(
                                                           () => SetConfigFlag(ChannelConfigChangeType.PlayOnLoad,
                                                               !IsPlayOnLoad)));

        public abstract bool IsPlayOnLoad { get; set; }
        public abstract bool IsAutoAdvance { get; set; }
        public abstract RepeatMode RepeatMode { get; set; }

        private TrackViewModel TrackAt(int index)
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
        protected abstract bool CanToggleConfig(ChannelConfigChangeType setting);

        /// <summary>
        ///     Implements a change of auto-advance to the given new value.
        /// </summary>
        /// <param name="setting">The setting to query.</param>
        /// <param name="newValue">The intended new value for auto-advance.</param>
        protected abstract void SetConfigFlag(ChannelConfigChangeType setting, bool newValue);
    }
}