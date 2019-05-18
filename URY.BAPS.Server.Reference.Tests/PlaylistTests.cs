using URY.BAPS.Common.Model.Track;
using URY.BAPS.Server.Reference.Model;
using Xunit;

namespace URY.BAPS.Server.Reference.Tests
{
    /// <summary>
    ///     Tests for <see cref="Playlist"/>.
    /// </summary>
    public class PlaylistTests
    {
        private readonly Playlist _playlist = new Playlist(0);

        /// <summary>
        ///     Adds a single library track to the playlist under test.
        /// </summary>
        private void AddOneItem()
        {
            _playlist.Add(new LibraryTrack("Beverly Hills", 90201));
        }
        
        /// <summary>
        ///     Tests that the length of a new playlist is zero.
        /// </summary>
        [Fact]
        public void TestLength_NewEmptyPlaylist()
        {
            Assert.Equal(0, _playlist.Length);
        }

        /// <summary>
        ///     Tests that the length of a playlist goes up after
        ///     adding a single item to an empty list.
        /// </summary>
        [Fact]
        public void TestLength_AfterAddToEmpty()
        {
            AddOneItem();
            Assert.Equal(1, _playlist.Length);
        }

        /// <summary>
        ///     Tests that the length of a playlist returns to zero after
        ///     adding a single item to an empty list, then removing it.
        /// </summary>
        [Fact]
        public void TestLength_AfterAddRemoveToEmpty()
        {
            AddOneItem();
            _playlist.Remove(0);
            Assert.Equal(0, _playlist.Length);
        }

        /// <summary>
        ///     Tests that the first item added to an empty playlist has
        ///     the index 0.
        /// </summary>
        [Fact]
        public void TestAddHasIndex_AddToEmpty()
        {
            AddOneItem();
            var track = _playlist.Get(0);
            var item = Assert.IsAssignableFrom<IPlaylistItem>(track);
            Assert.True(item.HasIndex(0));
        }
    }
}