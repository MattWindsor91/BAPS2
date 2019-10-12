using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Client.Protocol.V2.Controllers;
using URY.BAPS.Common.Model.MessageEvents;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel
{
    /// <summary>
    ///     The view model for a directory.
    /// </summary>
    public class DirectoryViewModel : ReactiveObject, IDirectoryViewModel
    {
        private readonly DirectoryController? _controller;

        private readonly ReadOnlyObservableCollection<DirectoryEntry> _files;

        private readonly ObservableAsPropertyHelper<string> _name;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();


        /// <summary>
        ///     Constructs a directory view model.
        /// </summary>
        /// <param name="directoryId">The server-assigned ID for this directory.</param>
        /// <param name="controller">The controller used to send refresh messages.</param>
        public DirectoryViewModel(ushort directoryId, DirectoryController? controller)
        {
            _controller = controller;
            DirectoryId = directoryId;

            var updater = controller?.Updater;
            ThisDirectoryFileAdd = updater == null
                ? Observable.Empty<DirectoryFileAddArgs>()
                : OnThisDirectory(updater.ObserveDirectoryFileAdd);
            ThisDirectoryPrepare = updater == null
                ? Observable.Empty<DirectoryPrepareArgs>()
                : OnThisDirectory(updater.ObserveDirectoryPrepare);

            Refresh = ReactiveCommand.Create(RefreshImpl);

            _subscriptions.Add(DirectoryChanges().ObserveOn(RxApp.MainThreadScheduler).Bind(out _files).Subscribe());
            _name = (from x in ThisDirectoryPrepare select x.Name).DefaultIfEmpty().ToProperty(this, x => x.Name);
        }

        private IObservable<DirectoryFileAddArgs> ThisDirectoryFileAdd { get; }
        private IObservable<DirectoryPrepareArgs> ThisDirectoryPrepare { get; }


        /// <summary>
        ///     The numeric server-side identifier of this directory.
        /// </summary>
        public ushort DirectoryId { get; }

        /// <summary>
        ///     The collection of files the server reports as being in this directory.
        /// </summary>
        public ReadOnlyObservableCollection<DirectoryEntry> Files => _files;

        /// <summary>
        ///     The human-readable name of this directory.
        /// </summary>
        public string Name => _name.Value;

        public void Dispose()
        {
            _name.Dispose();
            _subscriptions.Dispose();
            Refresh?.Dispose();
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
            where TResult : DirectoryArgsBase
        {
            return from ev in source where ev.DirectoryId == DirectoryId select ev;
        }

        /// <summary>
        ///     Converts this directory's incoming change observables into a
        ///     single <see cref="IChangeSet{DirectoryEntry}" /> observable.
        ///     <para>
        ///         This observer is then used to feed <see cref="Files" />,
        ///         which in turn feeds the UI view of the directory contents.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     An observable that reports changes in a directory in the form
        ///     of <see cref="IChangeSet{DirectoryEntry}" /> objects.
        /// </returns>
        private IObservable<IChangeSet<DirectoryEntry>> DirectoryChanges()
        {
            return ObservableChangeSet.Create<DirectoryEntry>(list =>
            {
                var adder = ThisDirectoryFileAdd.Subscribe(x =>
                    list.Insert((int) x.Index, new DirectoryEntry(x.DirectoryId, x.Description)));
                var clearer = ThisDirectoryPrepare.Subscribe(_ => list.Clear());
                return new CompositeDisposable(adder, clearer);
            });
        }

        #region Commands

        /// <summary>
        ///     A command that, when activated, sends a refresh request to the server.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Refresh { get; }

        /// <summary>
        ///     Executes a directory refresh command.
        /// </summary>
        private void RefreshImpl()
        {
            _controller?.Refresh();
        }

        #endregion Commands
    }
}