using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     Interface for view models representing local audio directories.
    /// </summary>
    public interface IDirectoryViewModel : IDisposable
    {
        /// <summary>
        ///     The human-readable name of this directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The numeric server-side identifier of this directory.
        /// </summary>
        ushort DirectoryId { get; }

        /// <summary>
        ///     A command that, when activated, sends a refresh request to the server.
        /// </summary>
        RelayCommand RefreshCommand { get; }

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        ObservableCollection<DirectoryEntry> Files { get; }
    }
}