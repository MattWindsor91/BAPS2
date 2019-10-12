using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
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
        ReactiveCommand<Unit, Unit> Refresh { get; }

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        ReadOnlyObservableCollection<DirectoryEntry> Files { get; }
    }
}