using System.Collections.ObjectModel;
using System.Reactive;
using JetBrains.Annotations;
using ReactiveUI;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.ViewModel.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="IDirectoryViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to channel controls when in design-time mode.
    ///     </para>
    /// </summary>
    public sealed class MockDirectoryViewModel : IDirectoryViewModel
    {
        /// <summary>
        ///     Constructs a mock directory view model with specific parameters.
        /// </summary>
        /// <param name="directoryId">The ID of this directory.</param>
        /// <param name="name">The name of this directory.</param>
        public MockDirectoryViewModel(ushort directoryId, string name)
        {
            DirectoryId = directoryId;
            Name = name;
            Files = new ReadOnlyObservableCollection<DirectoryEntry>(_files);

            Refresh = ReactiveCommand.Create(() => { });
        }

        /// <summary>
        ///     Constructs a mock directory view model with mocked-up parameters.
        /// </summary>
        [UsedImplicitly]
        public MockDirectoryViewModel() : this(0, "Jingles")
        {
            _files.Add(new DirectoryEntry(DirectoryId, "URY Whisper (Dry)"));
            _files.Add(new DirectoryEntry(DirectoryId, "Flagship News IN"));
            _files.Add(new DirectoryEntry(DirectoryId, "Salvation Tuesdays"));
            _files.Add(new DirectoryEntry(DirectoryId, "The More Beautiful Game (Promo)"));
        }

        public string Name { get; }
        public ushort DirectoryId { get; }
        public ReactiveCommand<Unit, Unit> Refresh { get; }
        public ReadOnlyObservableCollection<DirectoryEntry> Files { get; }
        private readonly ObservableCollection<DirectoryEntry> _files = new ObservableCollection<DirectoryEntry>();

        public void Dispose()
        {
            Refresh?.Dispose();
        }
    }
}