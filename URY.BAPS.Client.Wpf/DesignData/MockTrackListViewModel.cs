using URY.BAPS.Client.Common.Model;
using URY.BAPS.Client.Wpf.ViewModel;

namespace URY.BAPS.Client.Wpf.DesignData
{
    /// <summary>
    ///     A dummy implementation of <see cref="ITrackListViewModel" />.
    ///     <para>
    ///         This is used to provide sample data to channel and audio-wall controls in design mode.
    ///     </para>
    /// </summary>
    public class MockTrackListViewModel : TrackListViewModelBase
    {
        /// <summary>
        ///     Constructs a <see cref="MockTrackListViewModel" /> with a particular channel ID.
        /// </summary>
        /// <param name="channelId">The channel ID to use.</param>
        public MockTrackListViewModel(ushort channelId) : base(channelId)
        {
            Tracks.Add(new TrackViewModel(new FileTrack("URY Whisper (Dry)", 2_000)));
            Tracks.Add(new TrackViewModel(new LibraryTrack(
                    "Several Species of Small Furry Animals Gathered Together in a Cave and Grooving with a Pict",
                    36_000))
                {IsLoaded = true});
            Tracks.Add(new TrackViewModel(new TextTrack("Don't Panic", "Always remember where your towel is.")));
        }

        /// <summary>
        ///     Constructs a <see cref="MockTrackListViewModel" /> with a placeholder channel ID.
        /// </summary>
        public MockTrackListViewModel() : this(0)
        {
        }

        public override int SelectedIndex { get; set; } = -1;

        protected override bool CanLoadTrack(int index)
        {
            return true;
        }

        protected override void LoadTrack(int index)
        {
        }

        protected override bool CanResetPlaylist()
        {
            return true;
        }

        protected override void ResetPlaylist()
        {
        }

        protected override bool CanDeleteItem()
        {
            return true;
        }

        protected override void DeleteItem()
        {
        }

        protected override void DropDirectoryEntry(DirectoryEntry entry)
        {
        }

        protected override void DropText(string text)
        {            
        }

        public override void Dispose()
        {
            // Nothing to dispose
        }
    }
}