using JetBrains.Annotations;
using URY.BAPS.Client.Wpf.ViewModel;
using URY.BAPS.Common.Model.Track;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="DirectoryViewModelBase" />.
    ///     <para>
    ///         This is used to provide sample data to channel controls when in design-time mode.
    ///     </para>
    /// </summary>
    public sealed class MockDirectoryViewModel : DirectoryViewModelBase
    {
        /// <summary>
        ///     Constructs a mock directory view model with specific parameters.
        /// </summary>
        /// <param name="directoryId">The ID of this directory.</param>
        /// <param name="name">The name of this directory.</param>
        public MockDirectoryViewModel(ushort directoryId, string name) : base(directoryId)
        {
            Name = name;
        }

        /// <summary>
        ///     Constructs a mock directory view model with mocked-up parameters.
        /// </summary>
        [UsedImplicitly]
        public MockDirectoryViewModel() : this(0, "Jingles")
        {
            Files.Add(new DirectoryEntry(DirectoryId, "URY Whisper (Dry)"));
            Files.Add(new DirectoryEntry(DirectoryId, "Flagship News IN"));
            Files.Add(new DirectoryEntry(DirectoryId, "Salvation Tuesdays"));
            Files.Add(new DirectoryEntry(DirectoryId, "The More Beautiful Game (Promo)"));
        }

        public override string Name { get; protected set; }

        protected override void Refresh()
        {
        }

        protected override bool CanRefresh()
        {
            return true;
        }

        public override void Dispose()
        {
            // Nothing to dispose
        }
    }
}