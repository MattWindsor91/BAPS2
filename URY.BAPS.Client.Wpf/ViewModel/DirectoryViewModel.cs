using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using URY.BAPS.Client.Common.Controllers;
using URY.BAPS.Client.Common.Events;
using URY.BAPS.Client.Common.Model;

namespace URY.BAPS.Client.Wpf.ViewModel
{
    /// <summary>
    ///     The view model for a directory.
    /// </summary>
    public class DirectoryViewModel : ViewModelBase, IDisposable
    {
        [CanBeNull] private readonly DirectoryController _controller;

        [NotNull] private readonly IList<IDisposable> _subscriptions = new List<IDisposable>();
        private string _name;

        public DirectoryViewModel(ushort directoryId, [CanBeNull] DirectoryController controller)
        {
            _controller = controller;
            DirectoryId = directoryId;
            SubscribeToServerUpdates();
        }

        public ushort DirectoryId { get; }

        /// <summary>
        ///     The human-readable name of this directory.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        public ObservableCollection<DirectoryEntry> Files { get; } = new ObservableCollection<DirectoryEntry>();

        public void Dispose()
        {
            UnsubscribeFromServerUpdates();
        }

        /// <summary>
        ///     Restricts a directory observable to returning only events for this directory.
        /// </summary>
        /// <param name="source">The observable to filter.</param>
        /// <typeparam name="TResult">Type of output from the observable.</typeparam>
        /// <returns>The observable corresponding to filtering <see cref="source" /> to events for this directory.</returns>
        [NotNull]
        [Pure]
        private IObservable<TResult> OnThisDirectory<TResult>(IObservable<TResult> source)
            where TResult : DirectoryEventArgsBase
        {
            return from ev in source where ev.DirectoryId == DirectoryId select ev;
        }

        private void SubscribeToServerUpdates()
        {
            if (_controller == null) return;
            var updater = _controller.Updater;
            _subscriptions.Add(OnThisDirectory(updater.ObserveDirectoryFileAdd).Subscribe(HandleDirectoryFileAdd));
            _subscriptions.Add(OnThisDirectory(updater.ObserveDirectoryPrepare).Subscribe(HandleDirectoryPrepare));
        }

        private void UnsubscribeFromServerUpdates()
        {
            foreach (var subscription in _subscriptions) subscription.Dispose();
        }

        private void HandleDirectoryFileAdd(DirectoryFileAddEventArgs e)
        {
            var entry = new DirectoryEntry(DirectoryId, e.Description);
            DispatcherHelper.CheckBeginInvokeOnUI(() => Files.Insert((int) e.Index, entry));
        }

        /// <summary>
        ///     Handles a directory-prepare server update from the messenger bus.
        ///     <para>
        ///         Note that the main view model also listens for directory-prepare
        ///         updates; this is so that it can create
        ///         <see cref="DirectoryViewModel" />s for directory IDs that haven't
        ///         previously been used.
        ///     </para>
        /// </summary>
        /// <param name="e">The server update payload.</param>
        private void HandleDirectoryPrepare(DirectoryPrepareEventArgs e)
        {
            Name = e.Name;
            DispatcherHelper.CheckBeginInvokeOnUI(Files.Clear);
        }
    }
}