using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Interface for view models over channel track-lists.
    /// </summary>
    public interface ITrackListViewModel : IDropTarget
    {
        /// <summary>
        ///     The track list proper.
        /// </summary>
        [NotNull]
        ObservableCollection<TrackViewModel> Tracks { get; }

        /// <summary>
        ///     Gets the track at a particular index in the track list.
        /// </summary>
        /// <param name="index">The index of the track to retrieve, if possible.</param>
        /// <returns>
        ///     The track at <paramref name="index"/>, or a null track if one doesn't exist.
        /// </returns>
        [NotNull]
        TrackViewModel TrackAt(int index);

        /// <summary>
        ///     The currently-selected index.
        ///     <para>
        ///         This is not necessarily the index of the currently-loaded item;
        ///         selection and loading are subtly different (but related) states.
        ///     </para>
        /// </summary>
        int SelectedIndex { get; set; }
        
        /// <summary>
        ///     A command that, when fired, asks the server to reset the playlist.
        /// </summary>
        [NotNull]
        RelayCommand ResetPlaylistCommand { get; }

        /// <summary>
        ///     A command that, when fired, asks the server to delete the currently
        ///     selected item.
        /// </summary>
        [NotNull]
        RelayCommand DeleteItemCommand { get; }
        
        /// <summary>
        ///     A command that, when fired, asks the server to load the given item.
        /// </summary>       
        [NotNull]
        RelayCommand<int> LoadTrackCommand { get; }
    }
}