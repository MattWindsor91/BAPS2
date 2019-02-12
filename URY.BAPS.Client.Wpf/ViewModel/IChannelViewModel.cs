using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    public interface IChannelViewModel
    {
        /// <summary>
        ///     The part of the channel containing the loaded track and its
        ///     position markers.
        /// </summary>
        IPlayerViewModel Player { get; }

        /// <summary>
        ///     The name of the channel.
        ///     <para>
        ///         If not set manually, this is 'Channel X', where X
        ///         is the channel's ID plus one.
        ///     </para>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The track list.
        /// </summary>
        ObservableCollection<TrackViewModel> TrackList { get; }

        /// <summary>
        ///     A command that, when fired, checks the current auto advance
        ///     status and asks the server to invert it.
        /// </summary>
        RelayCommand ToggleAutoAdvanceCommand { get; }

        /// <summary>
        ///     A command that, when fired, checks the current play-on-load
        ///     status and asks the server to invert it.
        /// </summary>
        RelayCommand TogglePlayOnLoadCommand { get; }

        /// <summary>
        ///     Whether play-on-load is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        bool IsPlayOnLoad { get; set; }

        /// <summary>
        ///     Whether auto-advance is active, according to the server.
        ///     <para>
        ///         This property should only be set when the server state
        ///         changes.
        ///     </para>
        /// </summary>
        bool IsAutoAdvance { get; set; }

        RepeatMode RepeatMode { get; set; }
    }
}